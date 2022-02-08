using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;


namespace VDiary.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public UsersController(ApplicationDbContext context,IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _hasher = passwordHasher;
        }

        // GET: Users
        [Authorize(Roles= "Admin")]
        public async Task<IActionResult> Index()
        {
            var seed = new SeedForUser(_context);
            var applicationDbContext = _context.User.Include(u => u.Role);
            //seed.SeederAll(81000);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole != "Admin" && userId != id.ToString())
            {
                return RedirectToAction("Denied", "Home");
            }
            var user = await _context.User
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin,Lecturer")]
        public IActionResult Create()
        {
            IEnumerable<Role> items = _context.Role;

            if (items != null)
            {
                ViewBag.data = items;
            }
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Lecturer")
            {
                ViewBag.data = items.Where(r => r.Name == "Student");
            }
            

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Create([Bind("Surname,FirstName,Email,AlbumNumber,Signature,RoleId")] User user)
        {
            user.LastLoggedIn = DateTime.Now;
            user.DateCreated = DateTime.Now;
            user.DateResetRequest = DateTime.Now;
            user.AccountExpiryDays = user.DateCreated.Day - DateTime.Now.Day;
            user.IsDeleted = false;
            user.FilledLoginAtemps = 0;
            user.MaxLoginAttemps = 5;
            user.Password = _hasher.HashPassword(user, DefaultPassword()); 


            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Id", user.RoleId);
            return View(user);
        }

        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return RedirectToAction("Denied", "Home");
            }
            
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost, ActionName("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordPost(int? id,User user,string passwordR)
        {
            if (id is null)
            {
                return NotFound();
            }
            if (passwordR.Contains(" "))
            {
                TempData["Error"] = "Password can not contains spaces";
                user = _context.User.FirstOrDefault(u => u.Id == id);
                return View(user);
            }

            if (user.Password != passwordR)
            {
                TempData["Error"] = "Passwords must be the same ";
                user = _context.User.FirstOrDefault(u =>u.Id == id);
                return View(user);
            }

           
            var userToUpdate = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            userToUpdate.Password = _hasher.HashPassword(userToUpdate, user.Password);
            if (ModelState.IsValid)
            {
                _context.User.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = userToUpdate.Id });
            }
            return View(userToUpdate);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole != "Admin" && userId != id.ToString())
            {
                return RedirectToAction("Denied", "Home");
            }

            if (userRole != "Admin")
            {
                return RedirectToAction("ChangePassword", "Users",new {userId = id});
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userToUpdate = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            userToUpdate.Password = _hasher.HashPassword(userToUpdate, userToUpdate.Password);
            if (ModelState.IsValid)
            {
                _context.User.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = userToUpdate.Id });
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Id", userToUpdate.RoleId);
            return View(userToUpdate);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var user = await _context.User.FindAsync(id);

            if (user is null)
            {
                return NotFound();
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string DefaultPassword()
        {
            var bannedChars = new int[] { 39, 44, 46, 96 };
            var password = "";
            while (password.Length < 8)
            {
                Random RandomNumber = new Random();
                int newChar = RandomNumber.Next(33, 127);
                foreach (var cBan in bannedChars)
                {
                    if (newChar == cBan)
                    {
                        break;
                    }
                }
                password += (char)newChar;
            }

            return password;
        }
    }
}
