using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;
using VDiary.Models.Dtos;

namespace VDiary.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(int? id)
        {
            
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin")
            {
                var applicationDbContext = await _context.Course
                    .Include(c => c.Subject)
                    .Include(c => c.CourseUsers)
                    .ThenInclude(cu => cu.User)
                    .ToListAsync();
                var courses = _mapper.Map<List<CourseDto>>(applicationDbContext);


                return View(courses);
            }
            else
            {
                var applicationDbContext = _context.Course
                    .Include(c => c.Subject)
                    .Include(c => c.CourseUsers)
                    .ThenInclude(cu => cu.User)
                    .Where(c => c.CourseUsers.FirstOrDefault(cu => cu.UserId == id).UserId == userId).ToList();
                    ;
                
                var courses = _mapper.Map<List<CourseDto>>(applicationDbContext);

                return View(courses);
            }

            return NotFound();
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        private IActionResult RedirectToAction(Task<IActionResult> task)
        {
            throw new NotImplementedException();
        }

        // GET: Courses/Create
        [Authorize(Roles = "Lecturer,Admin")]
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> Create([Bind("SubjectId,LecturerId,Time,Venue,GroupName")] Course course)
        {
            var relation = new CourseUser();
            relation.CourseId = course.Id;
            relation.UserId = course.LecturerId;
            relation.Course = course;
            relation.User = _context.User.Find(course.LecturerId);
            if (course.Time < DateTime.Now)
            {
                course.Active = false;
            }
            else
            {
                course.Active = true;
            }


            if (ModelState.IsValid)
            {
                _context.Add(course);
                _context.CourseUser.Add(relation);
                await _context.SaveChangesAsync();
                //_context.SaveChanges();
                if (Content("Admin").Content == "Admin")
                {
                    return RedirectToAction(nameof(Index), new { id = 1});
                }
                return RedirectToAction(nameof(Index),new {id = course.LecturerId});
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View(course);
        }

        //GET :
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> CreateNewDate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Course
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> CreateNewDate(int id ,[Bind("Id,SubjectId,LecturerId,Time,Venue,GroupName,Active")] Course courseOld)
        {

            
            var newDate = courseOld.Time;
            courseOld = await _context.Course.FirstOrDefaultAsync(c => c.Id == courseOld.Id);
            if (courseOld.Time >= newDate)
            {
                TempData["Error"] = "New date have to latest than previous";
                return View(courseOld);
            }
            courseOld.Id = 0;
            var course = new Course();
            course = courseOld;
            course.Time = newDate;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);            
            var realations = _context.CourseUser.Where(cu => cu.CourseId == id);
            

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                var newId = _context.Course.OrderBy(c => c.Id).Last().Id;
                foreach (var courseUser in realations)
                {
                    courseUser.CourseId = newId;
                    courseUser.Id = 0;
                    _context.CourseUser.Add(courseUser);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index),new {id = userId});
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View(course);
        }
        // GET: Courses/Edit/5
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var courseDbContext =  await  _context.Course
                .Include(c => c.Subject)
                .Include(c => c.CourseUsers)
                .FirstOrDefaultAsync(m => m.Id == id);




            if (courseDbContext == null)
            {
                return NotFound();
            }
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View(courseDbContext);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseToupdate = await _context.Course.FirstOrDefaultAsync(c => c.Id == id);
            
            if (await TryUpdateModelAsync<Course>(
                courseToupdate,
                "",
                c => c.LecturerId,
                c => c.Time,
                c => c.Venue,
                c => c.Active
            ))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CourseExists(courseToupdate.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(Index),new {id = userId});
                }
            }

            ViewData["SubjectID"] = new SelectList(_context.Subject, "Id", "Id", courseToupdate.Subject);
            return View(courseToupdate);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new{id = course.LecturerId});
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }

        // GET: Courses/ShowMembers/5
        public async Task<IActionResult> ShowMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.CourseUser
                .Include(cu => cu.User)
                .Where(c => c.CourseId == id)
                .Where(c => c.User.RoleId != 2)
                .Select(u => u.User);

            if (course == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = id;
            return View(course);
        }
        
        //GET Courses/AddStudent/5 
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddStudent(int? id)  /// it is  course id 
        {
            if (id == null)
            {
                return NotFound();
            }

            var useres =  _context.User
                .Include(u =>u.CourseUsers)
                .ThenInclude(cu => cu.Course)
                .Where(u => u.RoleId != 1 && u.RoleId != 2)
                .Where(u => u.CourseUsers.All(cu => cu.CourseId != id))
                .ToList();


            ViewData["CourseId"] = id;
            return View(useres);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddStudent(int CourseId, int UserId)
        {
            var relation = new CourseUser();
            relation.CourseId = CourseId;
            relation.UserId = UserId;
            relation.Course = _context.Course.Find(CourseId);
            relation.User = _context.User.Find(UserId);

            if (CourseId == null)
            {
                return NotFound();
            }

            if (UserId == null)
            {
                return NotFound();
            }

            _context.CourseUser.Add(relation);
            _context.SaveChanges();

            return RedirectToAction("ShowMembers", new {id = CourseId});
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteStudentFromCourse(int id)
        {
            var course =_context.CourseUser
                .Where(cu => cu.UserId == id)
                .FirstOrDefault();
            _context.CourseUser.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowMembers", new { id = course.CourseId});
        }

       

    }    
}
