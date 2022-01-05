using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using VDiary.Data;
using VDiary.Models;

namespace VDiary.Controllers
{
    [Authorize(Roles = "Admin,Lecturer")]
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subject.ToListAsync());
        }


        // GET: Subjects/Create
        public IActionResult Create()
        {
           
            ViewData["Reffer"] = Request.Headers["Referer"].ToString();
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Subject subject,string? url)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                if (url is not null)
                {
                    return Redirect(url);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
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

            var subjectToUpdate = await _context.Subject.FirstOrDefaultAsync(s => s.Id == id);
            if (await TryUpdateModelAsync<Subject>(
                subjectToUpdate,
                "",
                s => s.Name
            ))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SubjectExists(subjectToUpdate.Id))
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
            }

            return View(subjectToUpdate);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subject.FindAsync(id);
            _context.Subject.Remove(subject);
            await _context.SaveChangesAsync();

            var relations = _context.SubjectUser.Where(su => su.SubjectId == id);
            foreach (var relation in relations)
            {
                _context.SubjectUser.Remove(relation);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subject.Any(e => e.Id == id);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var su = _context.SubjectUser
                .Include(su => su.User)
                .Include(su => su.Subject)
                .Where(su => su.BelongsTo == su.UserId)
                .Where(su => su.SubjectId == id )
                .Select(su => su.User)
                ;

            ViewData["Subject"] = _context.Subject.Find(id);

            if (su == null)
            {
                return NotFound();
            }

            return View(su);
        }
    }
}
