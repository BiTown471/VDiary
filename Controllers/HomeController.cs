using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;
using VDiary.Models.Dtos;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;

namespace VDiary.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public HomeController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _hasher = passwordHasher;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                
                return RedirectToAction("Index", "Courses", new { id = userId });
            }
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
            var condition = user.Password == "password";
            var result = _hasher.VerifyHashedPassword(user, user.Password, LoginUser.Password);


            if (result == PasswordVerificationResult.Success || condition)
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

        public IActionResult CreateAccount()
        {
            return View();
        } 
        [HttpPost]
        public IActionResult CreateAccount([Bind("FirstName,Surname,Email,AlbumNumber")] UserRegisterDto userDto)
        {

            if (userDto is null)
            {
                TempData["Error"] = "Something went wrong. Try Again ! ";

                return View("CreateAccount");
            }

            var exist = _context.User.Any(u => u.Email == userDto.Email || u.AlbumNumber == userDto.AlbumNumber);
            if (exist)
            {
                TempData["Error"] = "Account with this data is exist";

                return View("CreateAccount");
            }

            var newUser = new User();
            newUser.AlbumNumber = userDto.AlbumNumber;
            newUser.DateCreated = DateTime.Now;
            newUser.Email = userDto.Email;
            newUser.FirstName = userDto.FirstName;
            newUser.Surname = userDto.Surname;
            newUser.RoleId = 3;
            var newpassword = DefaultPassword();
            var hashPassword = _hasher.HashPassword(newUser, newpassword);
            newUser.Password = hashPassword;
           
            if (ModelState.IsValid)
            {
                _context.User.Add(newUser);
                _context.SaveChanges();
            }
            
            RegisterMail(newUser, newpassword);
            

            return View("Index");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword([Bind("Email,AlbumNumber")] UserRegisterDto userDto)
        {
            if (userDto is null)
            {
                TempData["Error"] = "Something went wrong. Try Again ! ";

                return View("ForgotPassword");
            }

            var exist = _context.User.Any(u => u.Email == userDto.Email && u.AlbumNumber == userDto.AlbumNumber);
            if (exist)
            {
                var user = _context.User.FirstOrDefault(u => u.Email == userDto.Email  || u.AlbumNumber == userDto.AlbumNumber);
                user.DateResetRequest = DateTime.Now;
                _context.User.Update(user);
                _context.SaveChanges();
                ResetMail(user);
                TempData["Success"] = "Email has been sent. Check your mail.";
            }
            else
            {
                TempData["Error"] = "Account with this data is exist";
            }
            return View("ForgotPassword");
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

        public IActionResult NewDefaultPassword(int? id, DateTime dateResetRequest)
        {
            if (id is null)
            {
                return NotFound();
            }

            var user = _context.User.FirstOrDefault(u => u.Id == id);

            if (user.DateResetRequest.ToString("s") == dateResetRequest.ToString("s"))
            {
                var newPassword = DefaultPassword();
                user.Password = _hasher.HashPassword(user, newPassword);
                RegisterMail(user, newPassword);
                _context.User.Update(user);
                _context.SaveChanges();
                TempData["Success"] = "Password has been changed. Check your mail.";
            }
            return View("Index");
        }

        private void RegisterMail(User user,string password)
        {
            var emailData = new AppData();
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("VDiary", emailData.mail));
            mail.To.Add(MailboxAddress.Parse(user.Email));
            mail.Subject = "Registration Mail From VDiary App";
            mail.Body = new TextPart("plain")
            {
                Text = $"Hello {user.FullName} \n \n  We are  glad that you are with Us \n \n " +
                       $"This is your password:\n {password} \n \n" 
            };

            SmtpClient smtpClient = new SmtpClient(); 
            
            try
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(emailData.mail, emailData.password);
                smtpClient.Send(mail);
                Console.WriteLine("Sended Registration mail ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {

                smtpClient.Disconnect(true);
                smtpClient.Dispose();
            }
        }

        private void ResetMail(User user)
        {
            var emailData = new AppData();
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("VDiary", emailData.mail));
            mail.To.Add(MailboxAddress.Parse(user.Email));
            mail.Subject = "Reset password From VDiary App";
            mail.Body = new TextPart("plain")
            {
                Text = $"Hello {user.FullName} \n \n  Have you send a request to reset your account password ? \n \n " +
                        "If yes click in link if not ignore this message  " +
                       "https://localhost:5001/Home/NewDefaultPassword?id=" + user.Id + "&dateResetRequest="+ user.DateResetRequest.ToString("s")
            };

            SmtpClient smtpClient = new SmtpClient();
            
            try
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(emailData.mail, emailData.password);
                smtpClient.Send(mail);
                Console.WriteLine("Sended Registration mail ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {

                smtpClient.Disconnect(true);
                smtpClient.Dispose();
            }
        }

     

    }

}
