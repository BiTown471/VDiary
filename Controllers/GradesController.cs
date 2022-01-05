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
using VDiary.Models.Dtos;

namespace VDiary.Controllers
{
    [Authorize]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Grades/IndexForLecturer/1
        public async Task<IActionResult> IndexForLecturer(int? pageNumber)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var pageSize = 5;

            var applicationDbContext = await _context.SubjectUser
                    .Where(g => g.BelongsTo == userId )
                .Select(su => su.Subject)
                .Distinct()
                .ToListAsync()
                ;

;
            var grades = new PaginatedList<Subject>(applicationDbContext, applicationDbContext.Count, pageNumber ?? 1, pageSize);
            ViewData["HasPreviousPage"] = grades.HasPreviousPage;
            ViewData["HasNextPage"] = grades.HasNextPage;
            ViewData["PageIndex"] = grades.PageIndex;
            return View(await PaginatedList<Subject>.Create(grades, pageNumber ?? 1, pageSize));
        }
        // GET: Grades/IndexForStudent/1
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> IndexForStudent(int? pageNumber)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["userFullName"] = User.FindFirstValue(ClaimTypes.Name);


            var pageSize = 5;

            var applicationDbContext = _context.Grade
                .Include(m => m.Subject)
                .Include(m => m.User)
                .Where( m => m.UserId == userId )
                .AsEnumerable()
                .ToList()
                ;

            ViewData["Subjects"] = await _context.SubjectUser
                .Include(su => su.User)
                .Include(su => su.Subject)
                .Where(su => su.UserId == userId)
                .Select(su => su.Subject)
                .ToListAsync();

            var grades = new PaginatedList<Grade>(applicationDbContext, applicationDbContext.Count, pageNumber ?? 1, pageSize);
            ViewData["HasPreviousPage"] = grades.HasPreviousPage;
            ViewData["HasNextPage"] = grades.HasNextPage;
            ViewData["PageIndex"] = grades.PageIndex;
            return View(await PaginatedList<Grade>.Create(grades, pageNumber ?? 1, pageSize));
        }


        // GET: Marks/Create
        [Authorize(Roles = "Lecturer")]
        public IActionResult CreateDefault()
        {

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var subjects =  _context.Course
                .Where(c => c.LecturerId == userId)
                .Select(c => c.Subject)
                ;

            var students = _context.SubjectUser
                .Where(su => su.BelongsTo == userId)
                .Where(su => su.User.Role.Name == "Student")
                .Select(su => su.User);

            ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name");
            ViewData["UserId"] = new SelectList(students, "Id", "FullName");
            return View();
        }
        // GET: Marks/Create/1
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create(int? id)// subjectID,
        {
            if (id is null)
            {
                return RedirectToAction("CreateDefault");
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var subject =  _context.Subject
                .FirstOrDefault(s => s.Id == id );

            var students = _context.SubjectUser
                .Where(su => su.BelongsTo == userId)
                .Where(su => su.User.Role.Name == "Student")
                .Where(s => s.Subject ==  subject)
                .Select(su => su.User);

            ViewData["SubjectId"] = subject.Id;
            ViewData["UserId"] = new SelectList(students, "Id", "FullName");
            return View();
        }


        // POST: Marks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create([Bind("GradeMark,Date,Description,UserId,SubjectId,CreatedBy")] Grade grade)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            grade.CreatedBy = userId;
            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction("AllGradeForSubject",new{ id = grade.SubjectId});
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", grade.SubjectId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "AlbumNumber", grade.UserId);
            return View(grade);
        }

        // GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Grade.FindAsync(id);
            if (mark == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", mark.SubjectId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "AlbumNumber", mark.UserId);
            return View(mark);
        }

        // POST: Marks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GradeMark,Date,Description,UserId,SubjectId")] Grade grade)
        {
            if (id != grade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(grade.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexForLecturer));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", grade.SubjectId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "AlbumNumber", grade.UserId);
            return View(grade);
        }

        // GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Grade
                .Include(m => m.Subject)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Grade.FindAsync(id);
            _context.Grade.Remove(mark);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexForLecturer));
        }

        private bool MarkExists(int id)
        {
            return _context.Grade.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AllGradeForSubject(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var lecturerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var subject = _context.Subject.FirstOrDefault(s => s.Id == id);
            var grades = await _context.Grade
                .Include(g => g.Subject)
                .Include(g => g.User)
                .Where(g => g.SubjectId == id)
                .Where(g => g.CreatedBy == lecturerId)
                .ToListAsync();

            ViewData["Subject"] = subject;

            ViewData["Students"]= await _context.User
                .Include(u => u.SubjectUser)
                .Where(u => u.SubjectUser.FirstOrDefault(su => su.BelongsTo== lecturerId ).BelongsTo == lecturerId)
                .Where(u => u.SubjectUser.FirstOrDefault(su => su.Subject.Name == subject.Name).Subject.Name == subject.Name)
                .Where(u => u.Role.Name == "Student")
                .ToListAsync()
                ;

            return View(grades);
        }

    }
}
