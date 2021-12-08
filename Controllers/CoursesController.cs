using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Index(int? id)
        {
            var user = _context.User
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);
            if (user is null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (user.Role.Name == "Admin")
            {
                var applicationDbContext = _context.Course
                    .Include(c => c.Subject)
                    .Include(c => c.Users)
                    .ToList();
                var courses = _mapper.Map<List<CourseDto>>(applicationDbContext);


                return View(courses);
            }
            else
            {
                var applicationDbContext = _context.Course
                    .Include(c => c.Subject)
                    .Include(c => c.Users)
                    .Where(c => c.Users.Any(u => u.Id == id))
                    .ToList();
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
        public async Task<IActionResult> Create([Bind("Id,SubjectId,LecturerId,Time,Venue,GroupName,Active")] Course course)
        {
            var relation = new CourseUser();
            relation.CourseId = course.Id;
            relation.UserId = course.LecturerId;
            relation.Course = course;
            relation.User = _context.User.Find(course.LecturerId);

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                _context.CourseUser.Add(relation);
                _context.SaveChanges();
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
            courseOld = _context.Course.FirstOrDefault(c => c.Id == courseOld.Id);
            courseOld.Id = 0;
            var course = new Course();
            course = courseOld;
            course.Time = newDate;

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
                return RedirectToAction(nameof(Index));
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
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.Id == id);


           // var course = _mapper.Map<CourseDto>(courseDbContext);

            if (courseDbContext == null)
            {
                return NotFound();
            }
            //ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name",course.Subject);
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View(courseDbContext);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public IActionResult Edit(int id, [Bind("Id,SubjectId,Subject,LecturerId,Time,Venue,GroupName,Active,Users,CourseUsers")] Course courseEdit)
        {
            if (id != courseEdit.Id)
            {
                return NotFound();
            }

            var course = _context.Course.Find(id);

            course.LecturerId = courseEdit.LecturerId;
            course.Time = courseEdit.Time;
            course.Venue = courseEdit.Venue;
            course.Active = courseEdit.Active;

            var relation = _context.CourseUser.First(cu => cu.CourseId == id);
            relation.UserId = courseEdit.LecturerId;
            relation.User = _context.User.Find(courseEdit.LecturerId);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectID"] = new SelectList(_context.Subject, "Id", "Id", course.Subject);
            return View(course);
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
        //GET
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var useres = _context.User
                .Where(u => u.RoleId != 1 && u.RoleId != 2).ToList();
            var mainCours = _context.Course.Find(id);
            var useres2 = _context.User
                .Include(u => u.Courses)
                .Where(u => !u.Courses.Contains(mainCours))
                .Where(u => u.RoleId != 1 && u.RoleId != 2);

            ViewData["CourseId"] = id;
            return View(useres2);
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

            //var useres = _context.User
            //   .Where(u => u.RoleId != 1 && u.RoleId != 2).ToList();

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
