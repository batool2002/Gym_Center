using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using System.Threading.Tasks;

namespace Fitness_Center.Controllers
{
    public class GuestsController : Controller
    {
        private readonly ModelContext _context;

        public GuestsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Home Page


        public async Task<IActionResult> Index()
        {
            var plans = await _context.Membershipplans.ToListAsync();
            var trainers = await _context.Trainers.Include(t => t.User).ToListAsync();

            ViewBag.Plans = plans;
            ViewBag.Trainers = trainers;

            return View();
        }

        public async Task<IActionResult> ViewTrainers()
        {
           
            var trainers = await _context.Trainers.Include(t => t.User).ToListAsync();
          
            ViewBag.Trainers = trainers;

            return View();
        }



        // GET: About Us Page
        public async Task<IActionResult> AboutUs()
        {
            var aboutUsPage = await _context.Staticpages.FirstOrDefaultAsync(p => p.PageName == "About Us");
            if (aboutUsPage == null)
            {
                return NotFound();
            }
            return View(aboutUsPage);
        }

        // GET: Contact Us Page
        public async Task<IActionResult> ContactUs()
        {
            var contactUsPage = await _context.Staticpages.FirstOrDefaultAsync(p => p.PageName == "Contact Us");
            if (contactUsPage == null)
            {
                return NotFound();
            }
            return View(contactUsPage);
        }

        // GET: View Membership Plans
        public async Task<IActionResult> ViewPlans()
        {
            var plans = await _context.Membershipplans.ToListAsync();
            return View(plans);
        }

        // GET: Testimonials
        public async Task<IActionResult> Testimonials()
        {
            var approvedTestimonials = await _context.Testimonials
                .Include(t => t.Member)
                .Where(t => t.Status == "Approved")
                .ToListAsync();

            return View(approvedTestimonials);
        }
    }
}
