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
    public class TicketTypesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TicketTypesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TicketTypes
        public async Task<IActionResult> Index()
        {
            var query = _context.TicketTypes.Include(t => t.Event).AsQueryable();

            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member != null)
                {
                    query = query.Where(t => t.Event.OrganizerId == member.MemberId);
                }
                else
                {
                    return View(new List<TicketType>());
                }
            }

            return View(await query.ToListAsync());
        }

        // GET: TicketTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.TicketTypeId == id);
            
            if (ticketType == null)
            {
                return NotFound();
            }

            // Ownership Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member == null || ticketType.Event.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
            }

            return View(ticketType);
        }

        // GET: TicketTypes/Create
        public async Task<IActionResult> Create(int? eventId)
        {
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                
                var eventsQuery = _context.Events.AsQueryable();
                if (member != null)
                {
                    eventsQuery = eventsQuery.Where(e => e.OrganizerId == member.MemberId);
                }
                
                // If eventId provided, verify ownership
                if (eventId.HasValue)
                {
                     var exists = await eventsQuery.AnyAsync(e => e.EventId == eventId);
                     if (!exists) return Forbid();
                }

                ViewData["EventId"] = new SelectList(eventsQuery, "EventId", "Title", eventId);
            }
            else
            {
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", eventId);
            }
            
            return View();
        }

        // POST: TicketTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketTypeId,TypeName,Price,SeatLimit,AvailableSeats,EventId")] TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                // Validate Ownership
                if (User.IsInRole("Organizer"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                    var evnt = await _context.Events.FindAsync(ticketType.EventId);
                    
                    if (member == null || evnt == null || evnt.OrganizerId != member.MemberId)
                    {
                        return Forbid();
                    }
                }

                _context.Add(ticketType);
                await _context.SaveChangesAsync();
                
                // Return to Event Details if possible, otherwise Index
                return RedirectToAction("Details", "Events", new { id = ticketType.EventId });
            }
            
            // Reload list with correct filter
             if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                 var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                 if (member != null)
                 {
                      ViewData["EventId"] = new SelectList(_context.Events.Where(e => e.OrganizerId == member.MemberId), "EventId", "Title", ticketType.EventId);
                 }
            }
            else
            {
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticketType.EventId);
            }

            return View(ticketType);
        }

        // GET: TicketTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes.Include(t => t.Event).FirstOrDefaultAsync(t => t.TicketTypeId == id);
            if (ticketType == null)
            {
                return NotFound();
            }

             // Ownership Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member == null || ticketType.Event.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
                 ViewData["EventId"] = new SelectList(_context.Events.Where(e => e.OrganizerId == member.MemberId), "EventId", "Title", ticketType.EventId);
            }
            else
            {
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticketType.EventId);
            }
            return View(ticketType);
        }

        // POST: TicketTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketTypeId,TypeName,Price,SeatLimit,AvailableSeats,EventId")] TicketType ticketType)
        {
            if (id != ticketType.TicketTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                     // Ownership Check
                    if (User.IsInRole("Organizer"))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                        var evnt = await _context.Events.FindAsync(ticketType.EventId); // Check new event ID if changed
                        var originalTicket = await _context.TicketTypes.AsNoTracking().Include(t => t.Event).FirstOrDefaultAsync(t => t.TicketTypeId == id);

                         // Check original and new
                        if (member == null || originalTicket.Event.OrganizerId != member.MemberId || (evnt != null && evnt.OrganizerId != member.MemberId))
                        {
                            return Forbid();
                        }
                    }

                    _context.Update(ticketType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketTypeExists(ticketType.TicketTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Events", new { id = ticketType.EventId });
            }
            
             // Reload list
             if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                 var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                 if (member != null)
                 {
                      ViewData["EventId"] = new SelectList(_context.Events.Where(e => e.OrganizerId == member.MemberId), "EventId", "Title", ticketType.EventId);
                 }
            }
            else
            {
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Title", ticketType.EventId);
            }
            return View(ticketType);
        }

        // GET: TicketTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.TicketTypeId == id);
            if (ticketType == null)
            {
                return NotFound();
            }

             // Ownership Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                 if (member == null || ticketType.Event.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
            }

            return View(ticketType);
        }

        // POST: TicketTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketType = await _context.TicketTypes.Include(t => t.Event).FirstOrDefaultAsync(t => t.TicketTypeId == id);
             // Ownership Check
            if (User.IsInRole("Organizer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                 if (member == null || ticketType.Event.OrganizerId != member.MemberId)
                {
                    return Forbid();
                }
            }

            if (ticketType != null)
            {
                _context.TicketTypes.Remove(ticketType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", new { id = ticketType.EventId });
        }

        private bool TicketTypeExists(int id)
        {
            return _context.TicketTypes.Any(e => e.TicketTypeId == id);
        }
    }
}
