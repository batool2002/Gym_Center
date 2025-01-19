using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fitness_Center.Controllers
{
    public class MembersController : Controller
    {
        private readonly ModelContext _context;

        public MembersController(ModelContext context)
        {
            _context = context;
        }

        // Helper method to get the logged-in MemberId from session
        private int? GetLoggedInMemberId()
        {
            if (HttpContext.Session.GetString("Role") == "Member" &&
                int.TryParse(HttpContext.Session.GetString("MemberId"), out int memberId))
            {
                return memberId;
            }
            return null;
        }

        // GET: Member Dashboard
        public async Task<IActionResult> Index()
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var member = await _context.Members
                .Include(m => m.Subscription)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: View Profile

        public async Task<IActionResult> ViewProfile()
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var member = await _context.Members
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Edit Profile
        public async Task<IActionResult> EditProfile()
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var member = await _context.Members
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile([Bind("MemberId,FullName,DateOfBirth,ContactNumber,Address,ProfilePicture")] Member member)
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null || memberId != member.MemberId)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var memberInDb = await _context.Members.FindAsync(memberId);
                    if (memberInDb == null)
                    {
                        return NotFound();
                    }

                    memberInDb.FullName = member.FullName;
                    memberInDb.DateOfBirth = member.DateOfBirth;
                    memberInDb.ContactNumber = member.ContactNumber;
                    memberInDb.Address = member.Address;
                    memberInDb.ProfilePicture = member.ProfilePicture;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(ViewProfile));
            }

            return View(member);
        }

        // GET: View Membership Plans
        public async Task<IActionResult> ViewPlans()
        {
            var plans = await _context.Membershipplans.ToListAsync();
            return View(plans);
        }

        // POST: Subscribe to a Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(decimal planId)
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var plan = await _context.Membershipplans.FindAsync(planId);
            if (plan == null)
            {
                return NotFound();
            }

            // Create a new subscription
            var subscription = new Subscription
            {
                MemberId = memberId.Value,
                PlanId = planId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths((int)plan.DurationMonths),
                PaymentStatus = "Unpaid"
            };

            _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();

            // Redirect to payment page
            return RedirectToAction("Pay", new { subscriptionId = subscription.SubscriptionId });
        }

        // GET: View Workouts
        public async Task<IActionResult> ViewWorkouts()
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            var workouts = await _context.Workouts
                .Where(w => w.MemberId == memberId)
                .Include(w => w.Trainer)
                .ToListAsync();

            return View(workouts);
        }

        // GET: Submit Testimonial
        public IActionResult SubmitTestimonial()
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            return View(new Testimonial());
        }

        // POST: Submit Testimonial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTestimonial([Bind("Content")] Testimonial testimonial)
        {
            var memberId = GetLoggedInMemberId();
            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegistration");
            }

            if (ModelState.IsValid)
            {
                testimonial.MemberId = memberId.Value;
                testimonial.Status = "Pending";
                testimonial.CreatedAt = DateTime.Now;

                _context.Testimonials.Add(testimonial);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(testimonial);
        }

        public async Task<IActionResult> Pay(int subscriptionId)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);

            var members = _context.Members
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.MemberId.ToString()
                })
                .ToList();

            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");

            if (subscription == null)
            {
                return NotFound();
            }

            var payment = new Payment
            {
                SubscriptionId = subscriptionId,
                Amount = subscription.Plan.Price
            };

            return View(payment);
        }

        // POST: Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay([Bind("MemberId,SubscriptionId,Amount,PaymentMethod")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                var memberId = GetLoggedInMemberId();
                if (memberId == null)
                {
                    return RedirectToAction("Login", "LoginAndRegistration");
                }

                

                // Check bank account balance
                var bankAccount = await _context.Bankaccounts.FirstOrDefaultAsync(b => b.MemberId == memberId);
                if (bankAccount == null || bankAccount.Balance < payment.Amount)
                {
                    ModelState.AddModelError("", "Insufficient balance in your bank account.");
                    return View(payment);
                }

                // Deduct the amount from the bank account
                bankAccount.Balance -= payment.Amount;

                payment.TransactionDate = DateTime.Now;
                _context.Payments.Add(payment);

                // Update subscription payment status
                var subscription = await _context.Subscriptions.FindAsync(payment.SubscriptionId);
                if (subscription != null)
                {
                    subscription.PaymentStatus = "Paid";
                }

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

            

            return View(payment);
        }
    }
}
