using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;
using TicketHub.Services;

namespace TicketHub.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPaymentService _paymentService;

        public PaymentsController(AppDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        // GET: Payments/Pay/5
        public async Task<IActionResult> Pay(int? bookingId)
        {
            if (bookingId == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            if (booking.BookingStatus == "Confirmed")
            {
                return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int bookingId, string paymentMethod)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return NotFound();

            if (booking.BookingStatus == "Confirmed")
            {
                 return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
            }

            var success = await _paymentService.ProcessPaymentAsync(booking.TotalAmount ?? 0, "USD", paymentMethod);

            if (success)
            {
                booking.BookingStatus = "Confirmed";
                _context.Update(booking);

                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = booking.TotalAmount,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "Completed",
                    TransactionReference = _paymentService.GenerateTransactionId()
                };
                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();

                return RedirectToAction("Success", new { id = booking.BookingId });
            }
            else
            {
                ModelState.AddModelError("", "Payment Failed. Please try again.");
                return View("Pay", booking);
            }
        }

        public IActionResult Success(int id)
        {
            return View(id);
        }
    }
}
