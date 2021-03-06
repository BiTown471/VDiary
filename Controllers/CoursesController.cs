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
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses/1
        public async Task<IActionResult> Index(int? pageNumber)
        {
            
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            int pageSize = 5;

            if (pageNumber is null)
            {
                pageNumber = 1;
            }
            var applicationDbContext = await _context.Course
                .Include(c => c.Subject)
                .Include(c => c.Lecturer)
                .OrderBy(c => c.Time)
                .ToListAsync();
            if (userRole == "Lecturer")
            {
                applicationDbContext = applicationDbContext
                    .Where(c => c.Lecturer.Id == userId)
                    .Distinct()
                    .ToList();
            }

            if (userRole == "Student")
            {
                applicationDbContext = await _context.Presence
                    .Where(p => p.UserId == userId)
                    .Select(p => p.Course)
                    .Distinct()
                    .OrderBy(c => c.Time)
                    .ToListAsync();
            }
            var pList = new PaginatedList<Course>(applicationDbContext, applicationDbContext.Count, pageNumber ?? 1, pageSize);
            ViewData["HasPreviousPage"] = pList.HasPreviousPage;
            ViewData["HasNextPage"] = pList.HasNextPage;
            ViewData["PageIndex"] = pList.PageIndex;
            return View(await PaginatedList<Course>.Create(applicationDbContext, pageNumber ?? 1, pageSize));
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
            var relation = new SubjectUser();
            relation.SubjectId = course.Id;
            relation.UserId = course.LecturerId;
            relation.Subject = _context.Subject.Find(course.SubjectId);
            relation.User = _context.User.Find(course.LecturerId);
            relation.BelongsTo = course.LecturerId;

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
                _context.SubjectUser.Add(relation);
                await _context.SaveChangesAsync();
                if (User.FindFirstValue(ClaimTypes.Role) == "Admin")
                {
                    return RedirectToAction(nameof(Index), new { id = 1});
                }
                return RedirectToAction(nameof(Index),new {id = course.LecturerId});
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");
            ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
            return View(course);
        }

        //GET : Courses/CreateNewDate/1
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> CreateNewDate(int? id) //previous course id 
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

        // POST: Courses/CreateNewDate/1
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
            course.Active = newDate < DateTime.Now;
            course.Time = newDate;
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var presences = _context.Presence.Where(p => p.CourseId == id);


            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                var newCourse = _context.Course.OrderBy(c => c.Id).Last();

                foreach (var presence in presences)
                {
                    presence.CourseId = newCourse.Id;
                    presence.Id = 0;
                    presence.Time = newCourse.Time;
                    presence.Active = false;
                    _context.Add(presence);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index),new {id = userId});
            }
           // ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");
           // ViewData["LecturerId"] = new SelectList(_context.User.Where(u => u.RoleId == 2), "Id", "FullName");
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
            var courseToUpdate = await _context.Course.FirstOrDefaultAsync(c => c.Id == id);
            
            if (await TryUpdateModelAsync<Course>(
                courseToUpdate,
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
                        if (!_context.Course.Any(e => e.Id == id))
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

            ViewData["SubjectID"] = new SelectList(_context.Subject, "Id", "Id", courseToUpdate.Subject);
            return View(courseToUpdate);
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
            _context.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new{id = course.LecturerId});
        }

        // GET: Courses/ShowMembers/5
        public async Task<IActionResult> ShowMembers(int? courseId, int? subjectId,int? lecturerId)
        {
            if (subjectId == null)
            {
                return NotFound();
            }

            if (lecturerId == null)
            {
                lecturerId = _context.Course.FirstOrDefault(c => c.Id == courseId).LecturerId;
            }

            var users = await _context.SubjectUser
                .Include(su => su.User)
                .Where(s => s.SubjectId == subjectId)
                .Where(s => s.User.RoleId != 2)
                .Where(s => s.BelongsTo == lecturerId)
                .Select(u => u.User)
                .ToListAsync();

            if (users == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = subjectId;
            ViewData["CourseId"] = courseId;
            return View(users);
        }
        
        //GET Courses/AddStudent/5 
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddStudent(int? id,int? courseId, string? filter)  /// it is  subject id 
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturer = _context.Course.FirstOrDefault(c => c.Id == courseId).LecturerId;

            var useres =  await _context.User
                .Include(u => u.SubjectUser)
                .Where(u => u.Role.Id == 3)
                .Where(u => u.SubjectUser.All(su => su.BelongsTo != lecturer))
                .ToListAsync();
            ViewData["Students"] = new SelectList(useres, "Id", "FullName"); 
            
            if (filter is not null)
            {
                useres = useres.Where(u => u.FullName.ToLower().Contains(filter.ToLower())).ToList();
            }

            ViewData["SubjectId"] = id;
            ViewData["CourseId"] = courseId;
            ViewData["lecturerId"] = lecturer;
            return View(useres);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddStudent(int subjectId, int userId,int courseId,[Bind("Id,FullName")] User user)
        {
            if (userId == 0)
            {
                userId = user.Id;
            }
            var relationSU = new SubjectUser();
            relationSU.SubjectId = subjectId;
            relationSU.UserId = userId;
            relationSU.Subject = _context.Subject.Find(subjectId);
            relationSU.User = _context.User.Find(userId);
            relationSU.BelongsTo = _context.Course.Find(courseId).LecturerId;

            var presence = new Presence()
            {
                Active = false,
                CourseId = courseId,
                Time = _context.Course.Find(courseId).Time,
                UserId = userId

            };
            var relations =  await _context.Course
                .Include(c => c.Subject)
                .Where(c => c.SubjectId == subjectId)
                .Where(c => c.Id != courseId)
                .ToListAsync()
                ;

            foreach (var relation in relations)
            {
                var p = new Presence()
                {
                    Active = false,
                    CourseId = relation.Id,
                    Time = _context.Course.Find(courseId).Time,
                    UserId = userId

                };
                _context.Add(p);
            }
            _context.SubjectUser.Add(relationSU);
            _context.Add(presence);
            _context.SaveChanges();

            return RedirectToAction("ShowMembers", new
            {
                courseId = courseId,
                subjectId = subjectId,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteStudentFromCourse(int id,int courseId,int subjectId)
        {
            var course = _context.SubjectUser
                .FirstOrDefault(su => su.UserId == id)
                ;
            var presences = _context.Presence
                    .Where(p => p.Course.SubjectId == subjectId)
                    .Where(p => p.UserId == id)
                ;
            foreach (var presence in presences)
            {
                _context.Remove(presence);
            }
            _context.SubjectUser.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowMembers", new
            {
                subjectId = course.SubjectId,
                courseId = courseId
            });
            
        }

       

    }    
}
