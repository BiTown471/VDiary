using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VDiary.Data;
using VDiary.Models;

namespace VDiary.Controllers
{
    public class PresencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Presences
        public async Task<IActionResult> Index()
        {
            return View(await _context.Presence.ToListAsync());
        }

        // GET: Presences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presence = await _context.Presence
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presence == null)
            {
                return NotFound();
            }

            return View(presence);
        }

        // GET: Presences/Create
        public IActionResult Create(int? courseId, int? subjectId, int? lecturerId)
        {
            if (subjectId == null)
            {
                return NotFound();
            }

            if (lecturerId == null)
            {
                lecturerId = _context.Course.FirstOrDefault(c => c.Id == courseId).LecturerId;
            }

            var presences = _context.Presence
                    .Include(p => p.User)
                    .Where(p => p.CourseId == courseId)
                    .ToList()
                ;
            

             return View(presences);
        }

        // POST: Presences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,UserId,Active,Course,User")] List<Presence> presences)
        {
            
            if (ModelState.IsValid)
            {
                foreach (var presence in presences)
                {
                    _context.Update(presence);

                }
                await _context.SaveChangesAsync();
                var course = _context.Course
                    .First(c => c.Id == presences.FirstOrDefault().CourseId)
                    
                    ;

                return RedirectToAction("PresencesForCourse",new
                {
                    courseId = course.Id,
                    subjectId = course.SubjectId
                });
            }
            return View(presences);
        }

        // GET: Presences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presence = await _context.Presence.FindAsync(id);
            if (presence == null)
            {
                return NotFound();
            }
            return View(presence);
        }

        // POST: Presences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,UserId,Active")] Presence presence)
        {
            if (id != presence.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(presence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresenceExists(presence.Id))
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
            return View(presence);
        }

        // GET: Presences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presence = await _context.Presence
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presence == null)
            {
                return NotFound();
            }

            return View(presence);
        }

        // POST: Presences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var presence = await _context.Presence.FindAsync(id);
            _context.Presence.Remove(presence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PresenceExists(int id)
        {
            return _context.Presence.Any(e => e.Id == id);
        }

        public async Task<IActionResult> PresencesForCourse(int? courseId , int? subjectId)
        {
            if (courseId is null)
            {
                return NotFound();
            }
            var lecturerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var subject = _context.Subject.FirstOrDefault(s => s.Id == subjectId);
            var presences = await _context.Presence
                    .Include(p => p.Course)
                    .Include(p => p.User)
                    .Where(p => p.CourseId == courseId)
                    .ToListAsync()
                    ;

            ViewData["Subject"] = subject;
            ViewData["CourseId"] = courseId;

            ViewData["Students"] = _context.User
                    .Include(u => u.SubjectUser)
                    .Where(u => u.SubjectUser.FirstOrDefault(su => su.BelongsTo == lecturerId).BelongsTo == lecturerId)
                    .Where(u => u.SubjectUser.FirstOrDefault(su => su.Subject.Name == subject.Name).Subject.Name == subject.Name)
                    .Where(u => u.Role.Name == "Student")
                ;

            return View(presences);
            //return View();
        }

        public IActionResult PresencesForSubject( int? subjectId)
        {
            var presents = _context.Presence
                .Include(p => p.Course)
                .Include(p => p.User)
                .Include(p => p.Course.Subject)
                .Where(p => p.Course.SubjectId == subjectId)
                .OrderBy(p => p.Time)
                ;
            
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Student")
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                presents = presents.Where(p => p.UserId == userId)
                    .OrderBy(p => p.Time);
                return View(presents);
            }
            return View(presents);
        }

        public async Task<IActionResult> PresencesForStudentAsync(int? pageNumber)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["userFullName"] = User.FindFirstValue(ClaimTypes.Name);


            var pageSize = 5;

            if (pageNumber is null)
            {
                pageNumber = 1;
            }
            var applicationDbContext = _context.SubjectUser
                    .Include(m => m.Subject)
                    .Include(m => m.User)
                    .Where(m => m.UserId == userId)
                    .AsEnumerable()
                    .ToList()
                ;


            var su = new PaginatedList<SubjectUser>(applicationDbContext, applicationDbContext.Count, pageNumber ?? 1, pageSize);
            ViewData["HasPeireviousPage"] = su.HasPreviousPage;
            ViewData["HasNextPage"] = su.HasNextPage;
            ViewData["PageIndex"] = su.PageIndex;
            return View(await PaginatedList<SubjectUser>.Create(su, pageNumber ?? 1, pageSize));
        }
    }
}
