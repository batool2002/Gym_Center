using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using System.Diagnostics;

namespace Fitness_Center.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ModelContext _context;

        public TrainerController(ModelContext context)
        {
            _context = context;
        }

        // GET: Trainer
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Trainers.Include(t => t.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Trainer/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainer/Create
      /*  public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Trainer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainerId,UserId,FullName,Specialization,ExperienceYears,ContactNumber,ProfilePicture")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
            return View(trainer);
        }

        // GET: Trainer/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
          //  Debug.WriteLine("id=" + id);
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
            return View(trainer);
        }

        // POST: Trainer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TrainerId,UserId,FullName,Specialization,ExperienceYears,ContactNumber,ProfilePicture")] Trainer trainer)
        {
            //Debug.WriteLine("ID=" + id);
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
            return View(trainer);
        }

        // GET: Trainer/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Trainers == null)
            {
                return Problem("Entity set 'ModelContext.Trainers'  is null.");
            }
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      */
        private bool TrainerExists(decimal id)
        {
          return (_context.Trainers?.Any(e => e.TrainerId == id)).GetValueOrDefault();
        }
    }
}
