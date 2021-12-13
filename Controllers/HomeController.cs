using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;

namespace VDiary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context )
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("/Denied")]
        public IActionResult Denied()
        {
            return View();
        }

        [HttpGet("/LoginUser")]
        public IActionResult LoginUser(string returnUrl)
        {

            return View("Index");
        }

        [HttpPost("/LoginUser"),ActionName("LoginUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUserPost([Bind("Email","Password")]User LoginUser)
        {
            
            var user = _context.User
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == LoginUser.Email);
            if (user is null)
            {
                TempData["Error"] = "Email or Password is invalid";

                return View("Index");
            }
            user.LastLoggedIn =DateTime.Now;
            user.AccountExpiryDays = (user.LastLoggedIn - user.DateCreated).Days + 1;
            if (user.Password == LoginUser.Password)
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email,user.Email));
                claims.Add(new Claim(ClaimTypes.Name, user.FullName));
                claims.Add(new Claim(ClaimTypes.Role,user.Role.Name));
                claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
                var claimsId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsId);
                await HttpContext.SignInAsync(claimsPrincipal);
                user.FilledLoginAtemps = 0;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Courses",new { id = user.Id});
            }
            user.DateFilledLoginAtemps = DateTime.Now;
            user.FilledLoginAtemps += 1;
            await _context.SaveChangesAsync();
            TempData["Error"] = "Email or Password is invalid";

            return View("Index");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
