using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;

namespace TicketHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard Metrics
            var totalEvents = await _context.Events.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalMembers = await _context.Members.CountAsync();
            var totalRevenue = await _context.Bookings
                .Where(b => b.BookingStatus != "Cancelled") // Assuming Cancelled status exists
                .SumAsync(b => b.TotalAmount) ?? 0;

            var recentBookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Member)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            ViewData["TotalEvents"] = totalEvents;
            ViewData["TotalBookings"] = totalBookings;
            ViewData["TotalMembers"] = totalMembers;
            ViewData["TotalRevenue"] = totalRevenue;

            return View(recentBookings);
        }
    }
}
