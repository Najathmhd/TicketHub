using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reviews.Include(r => r.Event).Include(r => r.Member);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Event)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public async Task<IActionResult> Create(int? eventId)
        {
            if (eventId == null)
            {
                return RedirectToAction("Index", "Events");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (member == null) return NotFound("Member record not found.");

            // Check if member has a booking for this event
            bool hasBooking = await _context.Bookings.AnyAsync(b => b.MemberId == member.MemberId && b.EventId == eventId);
            if (!hasBooking)
            {
                TempData["ErrorMessage"] = "You can only review events you have booked.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            ViewData["EventId"] = eventId;
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rating,Comment,EventId")] Review review)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (member == null)
            {
                return RedirectToAction("Register", "Account", new { area = "Identity" });
            }

            if (ModelState.IsValid)
            {
                // Re-verify booking in POST for security
                bool hasBooking = await _context.Bookings.AnyAsync(b => b.MemberId == member.MemberId && b.EventId == review.EventId);
                if (!hasBooking)
                {
                    TempData["ErrorMessage"] = "Unauthorized: You have not booked this event.";
                    return RedirectToAction("Details", "Events", new { id = review.EventId });
                }

                review.MemberId = member.MemberId;
                review.ReviewDate = DateOnly.FromDateTime(DateTime.Now);

                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Events", new { id = review.EventId });
            }
            ViewData["EventId"] = review.EventId;
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", review.EventId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", review.MemberId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,Rating,Comment,ReviewDate,MemberId,EventId")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", review.EventId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", review.MemberId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Event)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
