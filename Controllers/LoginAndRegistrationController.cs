using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Center.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Fitness_Center.Controllers
{
    public class LoginAndRegistrationController : Controller
    {
        private readonly ModelContext _context;

        public LoginAndRegistrationController(ModelContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Trainer)
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }







            // Store user data in session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Username", user.Username);

            // Redirect based on role
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admins");
            if (user.Role == "Trainer")
            {
                HttpContext.Session.SetString("TrainerId", user.Trainer.TrainerId.ToString());
                return RedirectToAction("Index", "Trainers");
            }
            if (user.Role == "Member")
            {
                HttpContext.Session.SetString("MemberId", user.Member.MemberId.ToString());
                return RedirectToAction("Index", "Members");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Register
        public IActionResult Register()
        {
            ViewData["Roles"] = new SelectList(new[] { "Admin", "Member", "Trainer", "Guest" });
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Password,Email,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
               // user.Role = "Member";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            ViewData["Roles"] = new SelectList(new[] { "Admin", "Member", "Trainer", "Guest" });
            return View(user);
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: ViewProfile
        public async Task<IActionResult> ViewProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(decimal.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: UpdateProfile
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(decimal.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("UserId,Username,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    user.UpdatedAt = DateTime.Now;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewProfile");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(u => u.UserId == user.UserId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(user);
        }
    }
}
