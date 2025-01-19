using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;

namespace Fitness_Center.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly ModelContext _context;

        public WorkoutsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Workouts.Include(w => w.Member).Include(w => w.Trainer);
            return View(await modelContext.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Member)
                .Include(w => w.Trainer)
                .FirstOrDefaultAsync(m => m.WorkoutId == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId");
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkoutId,TrainerId,MemberId,PlanName,Description,StartDate,EndDate")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workout.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workout.TrainerId);
            return View(workout);
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workout.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workout.TrainerId);
            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("WorkoutId,TrainerId,MemberId,PlanName,Description,StartDate,EndDate")] Workout workout)
        {
            if (id != workout.WorkoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.WorkoutId))
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workout.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workout.TrainerId);
            return View(workout);
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Member)
                .Include(w => w.Trainer)
                .FirstOrDefaultAsync(m => m.WorkoutId == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Workouts == null)
            {
                return Problem("Entity set 'ModelContext.Workouts'  is null.");
            }
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(decimal id)
        {
          return (_context.Workouts?.Any(e => e.WorkoutId == id)).GetValueOrDefault();
        }
    }
}
