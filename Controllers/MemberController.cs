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
    public class MemberController : Controller
    {
        private readonly ModelContext _context;

        public MemberController(ModelContext context)
        {
            _context = context;
        }

        // GET: Member
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Members.Include(m => m.Subscription).Include(m => m.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Subscription)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,UserId,FullName,DateOfBirth,ContactNumber,Address,SubscriptionId,ProfilePicture")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", member.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", member.UserId);
            return View(member);
        }

        // GET: Member/Edit/5
        public ActionResult Members()
        {
            var members = _context.Members.Include("User").ToList();
            return View(members);
        }

        public ActionResult Edit(decimal id)
        {
           
            var member = _context.Members.Find(id);
         //   if (member == null) return HttpNotFound();
            return View(member);
        }

        [HttpPost]
        public ActionResult Edit(Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(member).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Members");
            }
            return View(member);
        }

        public ActionResult Delete(decimal id)
        {
            var member = _context.Members.Find(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                _context.SaveChanges();
            }
            return RedirectToAction("Members");
        }
        private bool MemberExists(decimal id)
        {
          return (_context.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}
