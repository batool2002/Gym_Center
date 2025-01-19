using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.DependencyResolver;

namespace Fitness_Center.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public TrainersController(ModelContext context, IWebHostEnvironment webHostEnvironment )
        {
            _context = context;
            _webHostEnviroment = webHostEnvironment;
        }

        // Helper method to get the logged-in TrainerId from session
        private int? GetLoggedInTrainerId()
        {
            if (HttpContext.Session.GetString("Role") == "Trainer" &&
                int.TryParse(HttpContext.Session.GetString("TrainerId"), out int trainerId))
            {
                return trainerId;
            }
            return null;
        }

        // GET: Trainer Dashboard - List of Members
        /*
          public async Task<IActionResult> Index()
          {
              var trainerId = GetLoggedInTrainerId();
              if (trainerId == null)
              {
                  return RedirectToAction("Login", "LoginAndRegistration");
              }

              var members = await _context.Workouts
                  .Where(w => w.TrainerId == trainerId)
                  .Include(w => w.Member)
                  .Select(w => w.Member)
                  .Distinct()
                  .ToListAsync();

              return View(members);
          }
        */


        public async Task<IActionResult> Index()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var trainerId = GetLoggedInTrainerId();                                 // var trainer = await _context.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);

            var trainer = await _context.Trainers
      .Include(t => t.Workouts) // Include workouts if needed
      .Include(t => t.User) // Include user details
      .FirstOrDefaultAsync(t => t.UserId == userId && t.TrainerId == trainerId);


            if (trainer == null)
            {
                return NotFound();
            }

            // Fetch members assigned to the trainer
            var members = await _context.Members
                .Include(m => m.User)
                .Where(m => m.Workouts.Any(w => w.TrainerId == trainer.TrainerId)) // Members with this trainer's workouts
                .ToListAsync();

            return View(members); // Pass the list of members

            // return View(trainer);
        }
        // GET: Create Workout Plan
        /*
        public IActionResult CreateWorkoutPlan()
        {
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var members = _context.Members
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.FullName
                })
                .ToList();



            ViewBag.Members = new SelectList(members, "Value", "Text");


            return View(new Workout());
        }

        // POST: Create Workout Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan([Bind("MemberId,TrainerId,PlanName,Description,StartDate,EndDate")] Workout workout)
        {
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            if (ModelState.IsValid)
            {
                workout.TrainerId = trainerId.Value;
                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var members = _context.Members
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.FullName
                })
                .ToList();

            ViewBag.Members = new SelectList(members, "Value", "Text");
            return View(workout);
        }
        */


        public IActionResult CreateWorkoutPlan()
        {
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var members = _context.Members
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.MemberId.ToString()
                })
                .ToList();

            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId");
            return View(new Workout());
        }

        // POST: Create Workout Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan([Bind("MemberId,PlanName,Description,StartDate,EndDate")] Workout workout)
        {
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            if (ModelState.IsValid)
            {
                workout.TrainerId = trainerId.Value;
                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var members = _context.Members
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.MemberId.ToString()
                })
                .ToList();

            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", workout.MemberId);

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", workout.TrainerId);
            
        
            return View(workout);
        }



        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <returns></returns>


        // GET: View/Edit Profile
        public async Task<IActionResult> EditProfile()
        {
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }
                 

           
                var trainer = await _context.Trainers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.TrainerId == trainerId);

                if (trainer == null)
                {
                    return NotFound();
                }

                return View(trainer);
            }


        // POST: Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditProfile([Bind("TrainerId,FullName,Specialization,ExperienceYears,ContactNumber,ProfilePicture")] Trainer trainer)
        //{
        //    var trainerId = GetLoggedInTrainerId();
        //    if (trainerId == null || trainerId != trainer.TrainerId)
        //    {
        //        return RedirectToAction("Login", "LoginAndRegistration");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var trainerInDb = await _context.Trainers.FindAsync(trainerId);
        //            if (trainerInDb == null)
        //            {
        //                return NotFound();
        //            }

        //            trainerInDb.FullName = trainer.FullName;
        //            trainerInDb.Specialization = trainer.Specialization;
        //            trainerInDb.ExperienceYears = trainer.ExperienceYears;
        //            trainerInDb.ContactNumber = trainer.ContactNumber;
        //            trainerInDb.ProfilePicture = trainer.ProfilePicture;

        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            throw;
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(trainer);
        //}
        public async Task<IActionResult> EditProfile([Bind("FullName,Specialization,ExperienceYears,ContactNumber,ProfileFile")] Trainer trainer)
        {
            // Get the logged-in trainer's ID
            var trainerId = GetLoggedInTrainerId();
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Find the trainer record in the database
                    var trainerInDb = await _context.Trainers.FindAsync(trainerId);
                    if (trainerInDb == null)
                    {
                        return NotFound();
                    }


                 
                        /////Added
                        ///
                        if (trainer.ProfileFile != null)
                        {
                            string wwwRootPath = _webHostEnviroment.WebRootPath;
                            string fileName = Guid.NewGuid().ToString() + "_" +
                            trainer.ProfileFile.FileName;
                            string path = Path.Combine(wwwRootPath + "/Images/",
                            fileName);
                            using (var fileStream = new FileStream(path,
                            FileMode.Create))
                            {
                                await trainer.ProfileFile.CopyToAsync(fileStream);
                            }
                            trainer.ProfilePicture = fileName;
                        }


                        // Update the trainer's profile information
                        trainerInDb.FullName = trainer.FullName;
                        trainerInDb.Specialization = trainer.Specialization;
                        trainerInDb.ExperienceYears = trainer.ExperienceYears;
                        trainerInDb.ContactNumber = trainer.ContactNumber;
                        trainerInDb.ProfilePicture = trainer.ProfilePicture;

                        // Save changes to the database
                        await _context.SaveChangesAsync();
                    }
                
                catch (DbUpdateConcurrencyException)
                {
                    throw; // Let higher-level middleware handle this
                }

                // Redirect to the trainer's index/dashboard
                return RedirectToAction(nameof(Index));
            }
        

            // If validation fails, return the view with the same model
            return View(trainer);
        }







        private decimal GetLoggedInUserId()
        {

            // Assuming a simple method to retrieve the logged-in user's ID
            var userIdString = HttpContext.Session.GetString("UserId");
            return decimal.Parse(userIdString);

        }

        // Remaining actions (EditWorkoutPlan, EditProfile, etc.)...
        // Follow the same pattern to enforce authentication and session validation.
    }
}
