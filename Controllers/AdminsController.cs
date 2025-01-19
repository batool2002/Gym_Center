using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.DependencyResolver;
using System.Diagnostics.Metrics;

namespace Fitness_Center.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ModelContext _context;

        public AdminsController(ModelContext context)
        {
            _context = context;
        }

        private int? GetLoggedInAdminId()
        {
            if (HttpContext.Session.GetString("Role") == "Admin" &&
                int.TryParse(HttpContext.Session.GetString("AdminId"), out int AdminId))
            {
                return AdminId;
            }
            return null;
        }

        // GET: Admin Dashboard - Key statistics
        public async Task<IActionResult> Index()
        {

            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var trainerId = GetLoggedInAdminId();

            var memberCount = await _context.Members.CountAsync();
            var activeSubscriptions = await _context.Subscriptions.CountAsync(s => s.PaymentStatus == "Paid");
            var totalRevenue = await _context.Payments.SumAsync(p => p.Amount);

            ViewBag.MemberCount = memberCount;
            ViewBag.ActiveSubscriptions = activeSubscriptions;
            ViewBag.TotalRevenue = totalRevenue;

            return View();
        }

        // GET: Manage Members
        public async Task<IActionResult> ManageMembers()
        {
           
            var members = _context.Members.Include(m => m.User).Where(m => m.User != null).Include(m => m.Subscription).ToList();

            return View(members);
        }

        // GET: Create Member

        public IActionResult CreateMember()
        {
            var users = _context.Users       
        .Where(u => !_context.Members.Any(m => m.UserId == u.UserId))
        .Select(u => new SelectListItem
        {
            Value = u.UserId.ToString(),
            Text = u.Username

        })
        .ToList();

            var Subscription = _context.Subscriptions
                 .Select(s => new SelectListItem
                 {
                     Value = s.SubscriptionId.ToString(),
                     Text = s.SubscriptionId.ToString()

                 })
        .ToList();

            ViewBag.Users = new SelectList(users, "Value", "Text");
            ViewBag.Subscription = new SelectList(Subscription, "Value", "Text");
            // ViewBag.Subsicriptions = new SelectList(Subscription, "Value", "Text");
            // ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", member.UserId);
            return View(new Member());
        }

        // POST: Create Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember([Bind("UserId,SubscriptionId,FullName, DateOfBirth, ContactNumber, Address")] Member member)
        {

            if (!ModelState.IsValid)
            {

                var users = _context.Users
                    .Where(u => !_context.Members.Any(m => m.UserId == u.UserId))
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    })
            .ToList();

  

                _context.Members.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageMembers));

                //ViewBag.Users = new SelectList(users, "Value", "Text");
                //return View(trainer);
            }
             ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", member.UserId);

            var Subscription = _context.Subscriptions
   .Select(s => new SelectListItem
   {
       Value = s.SubscriptionId.ToString(),
       Text = s.SubscriptionId.ToString()

   })
.ToList();
            ViewBag.Subscription = new SelectList(Subscription, "Value", "Text");
             ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", member.SubscriptionId);
            //  ViewBag.Users = new SelectList(users, "Value", "Text");

            return View(member);



        }


        // GET: Edit Member
        public async Task<IActionResult> EditMember(int id)
        {
            // Retrieve the member with the specified ID, including associated User and Subscription
            var member = await _context.Members
                .Include(m => m.User)
                .Include(m => m.Subscription)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            // Get the list of users who are not already members (so they can be assigned to the new member)
            var users = _context.Users
                .Where(u => u.UserId == member.UserId || !_context.Members.Any(m => m.UserId == u.UserId)) // Allow the current UserId to be selected
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Username
                })
                .ToList();

            // Get the list of subscriptions
            var subscriptions = _context.Subscriptions
                .Select(s => new SelectListItem
                {
                    Value = s.SubscriptionId.ToString(),
                    Text = s.SubscriptionId.ToString() // Assuming SubscriptionName is more meaningful
                })
                .ToList();

            // Pass the data to the view
            ViewBag.Users = new SelectList(users, "Value", "Text", member.UserId);
            ViewBag.Subscription = new SelectList(subscriptions, "Value", "Text", member.SubscriptionId);

            return View(member);
        }

        // POST: Edit Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(int id, [Bind("MemberId, UserId, SubscriptionId, FullName, DateOfBirth, ContactNumber, Address")] Member member)
        {
            // Check if the provided ID matches the one from the form
            if (id != member.MemberId)
            {
                return NotFound();
            }

            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Repopulate the users and subscriptions dropdowns if model is invalid
                var users = _context.Users
                    .Where(u => u.UserId == member.UserId || !_context.Members.Any(m => m.UserId == u.UserId))
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    })
                    .ToList();

                var subscriptions = _context.Subscriptions
                    .Select(s => new SelectListItem
                    {
                        Value = s.SubscriptionId.ToString(),
                        Text = s.SubscriptionId.ToString() // Use a more meaningful display name for Subscription
                    })
                    .ToList();

                // Repopulate the dropdown lists in the view
                ViewBag.Users = new SelectList(users, "Value", "Text", member.UserId);
                ViewBag.Subscription = new SelectList(subscriptions, "Value", "Text", member.SubscriptionId);

                // Return the view with validation errors
                return View(member);
            }

            try
            {
                // Retrieve the existing member from the database
                var memberInDb = _context.Members.SingleOrDefault(m => m.MemberId == id);

                if (memberInDb == null)
                {
                    return NotFound();
                }

                // Update the member properties in the database
                memberInDb.UserId = member.UserId;
                memberInDb.SubscriptionId = member.SubscriptionId;
                memberInDb.FullName = member.FullName;
                memberInDb.DateOfBirth = member.DateOfBirth;
                memberInDb.ContactNumber = member.ContactNumber;
                memberInDb.Address = member.Address;
                memberInDb.Address = member.ProfilePicture;

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle any concurrency issues (e.g., if the member was modified by someone else)
                if (!MemberExists(member.MemberId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to the ManageMembers page after successful edit
            return RedirectToAction(nameof(ManageMembers));
        }

        private bool MemberExists(decimal memberId)
        {
            throw new NotImplementedException();
        }



        // GET: Delete Member
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Delete Member
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMemberConfirmed(int id)
        {
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member != null)
            {
                _context.Users.Remove(member.User);
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageMembers));
        }

        // GET: Manage Trainers


        public ActionResult ManageTrainers()
        {
            var trainers = _context.Trainers.Include(t => t.User).Where(t => t.User != null).ToList();

            return View(trainers);
        }



        public ActionResult CreateTrainer()
        {
            var users = _context.Users
                .Where(u => !_context.Trainers.Any(t => t.UserId == u.UserId))
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Username

                })
                .ToList();

           
                ViewBag.Users = new SelectList(users, "Value", "Text");
           
            return View(new Trainer());
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainer([Bind("UserId, FullName, Specialization, ExperienceYears, ContactNumber, ProfilePicture")] Trainer trainer)
        {
            
            if (!ModelState.IsValid)
            {


                // Re-populate the Users dropdown in case of validation errors
             //   if (ViewBag.Users.UserId == null) { return RedirectToAction(nameof(Index)); }
                var users = _context.Users
                    .Where(u => !_context.Trainers.Any(t => t.UserId == u.UserId))
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    })
                    .ToList();

                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTrainers));

                //ViewBag.Users = new SelectList(users, "Value", "Text");
                //return View(trainer);
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
          //  ViewBag.Users = new SelectList(users, "Value", "Text");

            return View(trainer);
        }



        // GET: Edit Trainer
        public ActionResult EditTrainer(int id)
        {
            var trainer = _context.Trainers.SingleOrDefault(t => t.TrainerId == id);
            if (trainer == null) return NotFound();

            var users = _context.Users
                .Where(u => u.UserId == trainer.UserId || !_context.Trainers.Any(t => t.UserId == u.UserId))
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Username
                })
                .ToList();
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
            // ViewBag.Users = new SelectList(users, "Value", "Text", trainer.UserId);
            return View(trainer);
        }

        // POST: Edit Trainer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer([Bind("TrainerId, UserId, FullName, Specialization, ExperienceYears, ContactNumber, ProfilePicture")] Trainer trainer)
        {
            if (!ModelState.IsValid)
            {
                var users = _context.Users
                    .Where(u => u.UserId == trainer.UserId || !_context.Trainers.Any(t => t.UserId == u.UserId))
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    })
                    .ToList();

                var trainerInDb = _context.Trainers.Single(t => t.TrainerId == trainer.TrainerId);
                trainerInDb.FullName = trainer.FullName;
                trainerInDb.Specialization = trainer.Specialization;
                trainerInDb.ExperienceYears = trainer.ExperienceYears;
                trainerInDb.ContactNumber = trainer.ContactNumber;
                trainerInDb.ProfilePicture = trainer.ProfilePicture;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTrainers));

                // ViewBag.Users = new SelectList(users, "Value", "Text", trainer.UserId);


            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", trainer.UserId);
            return View(trainer);
        }



        public async Task<IActionResult> DeleteTrainer(int id)
        {
            // Find the trainer by their id
            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.TrainerId == id);

            // If the trainer is not found, return a 404 error
            if (trainer == null)
            {
                return NotFound();
            }

            // Pass the trainer to the view
            return View(trainer);
        }

        [HttpPost, ActionName("DeleteTrainer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainerConfirmed(int id)
        {
            // Find the trainer by their id
            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.TrainerId == id);

            // If the trainer is not found, return a 404 error
            if (trainer == null)
            {
                return NotFound();
            }

            // Remove the trainer from the context and save changes
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();

            // Redirect to the ManageTrainers page after the deletion
            return RedirectToAction(nameof(ManageTrainers));
        }



        private bool TrainerExists(decimal trainerId)
        {
            throw new NotImplementedException();
        }

        // Approve or Reject Testimonials
        public async Task<IActionResult> ManageTestimonials()
        {
            var testimonials = await _context.Testimonials.Include(t => t.Member).ToListAsync();
            return View(testimonials);
        }

        // Approve Testimonial
        public async Task<IActionResult> ApproveTestimonial(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTestimonials));
        }



        // Reject Testimonial
        public async Task<IActionResult> RejectTestimonial(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTestimonials));
        }

        // Manage Static Pages
        public async Task<IActionResult> ManageStaticPages()
        {
            var pages = await _context.Staticpages.ToListAsync();
            return View(pages);
        }

        // Edit Static Page
        public async Task<IActionResult> EditStaticPage(int id)
        {
            var page = await _context.Staticpages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Update Static Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaticPage(int id, [Bind("PageId, PageName, Content")] Staticpage page)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    page.UpdatedAt = DateTime.Now;
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Staticpages.Any(p => p.PageId == page.PageId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(ManageStaticPages));
            }
            return View(page);
        }

        // View Reports
        public async Task<IActionResult> ViewReports()
        {
            var reports = await _context.Reports.ToListAsync();
            return View(reports);
        }

        private decimal GetLoggedInUserId()
        {

            // Assuming a simple method to retrieve the logged-in user's ID
            var userIdString = HttpContext.Session.GetString("UserId");
            return decimal.Parse(userIdString);

        }
    }
}
