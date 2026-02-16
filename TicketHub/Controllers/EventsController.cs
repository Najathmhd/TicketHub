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
    [Authorize(Roles = "Admin,Organizer")]
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events/MyEvents
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> MyEvents()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (member == null) return NotFound("Member not found.");

            var myEvents = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Bookings)
                .Where(e => e.OrganizerId == member.MemberId)
                .ToListAsync();

            return View(myEvents);
        }

        // GET: Events
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? venueId)
        {
            var appDbContext = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                appDbContext = appDbContext.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                appDbContext = appDbContext.Where(s => s.CategoryId == categoryId);
            }

             if (venueId.HasValue)
            {
                appDbContext = appDbContext.Where(s => s.VenueId == venueId);
            }

             ViewData["CategoryId"] = new SelectList(_context.EventCategories, "CategoryId", "CategoryName", categoryId);
             ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", venueId);
             ViewData["CurrentFilter"] = searchString;

            return View(await appDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evnt = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (evnt == null)
            {
                return NotFound();
            }

            return View(evnt);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.EventCategories, "CategoryId", "CategoryName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Title,Description,EventDate,EventTime,EventStatus,CategoryId,VenueId")] Event evnt)
        {
            if (ModelState.IsValid)
            {
                // Set Organizer
                var user = await _userManager.GetUserAsync(User);
                 if (user != null)
                {
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                    if (member != null)
                    {
                        evnt.OrganizerId = member.MemberId;
                    }
                }

                _context.Add(evnt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.EventCategories, "CategoryId", "CategoryName", evnt.CategoryId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evnt.VenueId);
            return View(evnt);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evnt = await _context.Events.FindAsync(id);
            if (evnt == null)
            {
                return NotFound();
            }

            // Authorization Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member == null || evnt.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.EventCategories, "CategoryId", "CategoryName", evnt.CategoryId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evnt.VenueId);
            return View(evnt);
        }

        // POST: Events/Edit/5
        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,EventDate,EventTime,EventStatus,CategoryId,VenueId")] Event evnt)
        {
            if (id != evnt.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);
                     // Authorization Check
                    if (User.IsInRole("Organizer"))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                        if (existingEvent == null || member == null || existingEvent.OrganizerId != member.MemberId)
                        {
                            return Forbid();
                        }
                    }

                    // Preserve OrganizerId
                    evnt.OrganizerId = existingEvent.OrganizerId;

                    _context.Update(evnt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   // ... existing catch logic
                    if (!_context.Events.Any(e => e.EventId == id))
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

            ViewData["CategoryId"] = new SelectList(_context.EventCategories, "CategoryId", "CategoryName", evnt.CategoryId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evnt.VenueId);
            return View(evnt);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evnt = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (evnt == null)
            {
                return NotFound();
            }

            // Authorization Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member == null || evnt.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
            }

            return View(evnt);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evnt = await _context.Events.FindAsync(id);
            if (evnt != null)
            {
                // Authorization Check
                if (User.IsInRole("Organizer"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                    if (member == null || evnt.OrganizerId != member.MemberId)
                    {
                        return Forbid();
                    }
                }

                _context.Events.Remove(evnt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
