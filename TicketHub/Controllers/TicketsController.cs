using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;

namespace TicketHub.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetTicket(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Member)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.TicketType)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            // Authorization
            if (!User.IsInRole("Admin") && booking.Member.Email != User.Identity.Name)
            {
                return Forbid();
            }

            if (booking.BookingStatus != "Confirmed")
            {
                return BadRequest("Ticket is not confirmed yet.");
            }

            return View(booking);
        }
    }
}
