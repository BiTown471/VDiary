using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles= "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.User.Include(u => u.Role);

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

           // ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Id");
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
            user.Password = DefaultPassword();


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
        public async Task<IActionResult> ChangePasswordPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userToUpdate = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (ModelState.IsValid)
            {
                if (await TryUpdateModelAsync<User>(
                    userToUpdate,
                    "",
                    u => u.Password                    
                    )                    
                )
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(userToUpdate.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return RedirectToAction("Details", new { id = userToUpdate.Id });
            }
           // ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Id", userToUpdate.RoleId);
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
            if (ModelState.IsValid)
            {
                if (await TryUpdateModelAsync<User>(
                    userToUpdate,
                    "",
                    u => u.FirstName,
                    u => u.Surname,
                    u => u.Password,
                    u => u.Email,
                    u => u.IsDeleted,
                    u => u.Signature,
                    u => u.Role
                ))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(userToUpdate.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                
                return RedirectToAction("Details",new {id = userToUpdate.Id});
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

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
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
