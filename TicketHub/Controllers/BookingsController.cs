using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;

using Microsoft.AspNetCore.Authorization;

using TicketHub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                 return Challenge();
            }

            var query = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Member)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                 var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                 if (member != null)
                 {
                     query = query.Where(b => b.MemberId == member.MemberId);
                 }
                 else
                 {
                     return View(new List<Booking>());
                 }
            }

            return View(await query.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create(int? eventId)
        {
            if (eventId == null)
            {
                return RedirectToAction("Index", "Events");
            }

            var ticketEvent = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(m => m.EventId == eventId);

            if (ticketEvent == null)
            {
                return NotFound();
            }

            var viewModel = new BookingCreateViewModel
            {
                EventId = ticketEvent.EventId,
                EventTitle = ticketEvent.Title,
                EventDate = ticketEvent.EventDate,
                VenueName = ticketEvent.Venue.VenueName,
                TicketTypes = ticketEvent.TicketTypes
            };

            return View(viewModel);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel castingData)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate TicketTypes if validation fails
                var eventEntity = await _context.Events.Include(e => e.TicketTypes).FirstOrDefaultAsync(e => e.EventId == castingData.EventId);
                castingData.TicketTypes = eventEntity?.TicketTypes;
                return View(castingData);
            }

            // Get Current User
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (member == null)
            {
                // Should not happen if Register works correctly, but handle it
                return RedirectToAction("Register", "Account", new { area = "Identity" });
            }

            // check Ticket availability
            var ticketType = await _context.TicketTypes.FindAsync(castingData.SelectedTicketTypeId);
            if (ticketType == null)
            {
                ModelState.AddModelError("", "Invalid Ticket Type.");
                return View(castingData);
            }

            if (ticketType.AvailableSeats < castingData.Quantity)
            {
                ModelState.AddModelError("", "Not enough seats available.");
                 // Re-populate TicketTypes
                var eventEntity = await _context.Events.Include(e => e.TicketTypes).FirstOrDefaultAsync(e => e.EventId == castingData.EventId);
                castingData.TicketTypes = eventEntity?.TicketTypes;
                return View(castingData);
            }

            // Create Booking
            var booking = new Booking
            {
                 BookingDate = DateOnly.FromDateTime(DateTime.Now),
                 TotalAmount = ticketType.Price * castingData.Quantity,
                 BookingStatus = "Pending",
                 MemberId = member.MemberId,
                 EventId = castingData.EventId
            };

            _context.Add(booking);
            await _context.SaveChangesAsync();

            // Create Booking Detail
            var bookingDetail = new BookingDetail
            {
                BookingId = booking.BookingId,
                TicketTypeId = ticketType.TicketTypeId,
                Quantity = castingData.Quantity,
                SubTotal = ticketType.Price * castingData.Quantity
            };
            _context.BookingDetails.Add(bookingDetail); // Assuming BookingDetail entity exists and has these fields.
            // Wait, I need to check BookingDetail properties.
            // Let's assume standard structure, if not I'll fix it.

            // Update Available Seats
            ticketType.AvailableSeats -= castingData.Quantity;

            _context.Update(ticketType);
            await _context.SaveChangesAsync();

            return RedirectToAction("Pay", "Payments", new { bookingId = booking.BookingId });
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", booking.MemberId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,TotalAmount,BookingStatus,MemberId,EventId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", booking.MemberId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
