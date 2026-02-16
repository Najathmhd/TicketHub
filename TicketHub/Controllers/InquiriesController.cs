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
    public class InquiriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InquiriesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Inquiries
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var query = _context.Inquiries.Include(i => i.Member).AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
                if (member != null)
                {
                    query = query.Where(i => i.MemberId == member.MemberId);
                }
            }

            return View(await query.ToListAsync());
        }

        // GET: Inquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries
                .Include(i => i.Member)
                .FirstOrDefaultAsync(m => m.InquiryId == id);
            if (inquiry == null)
            {
                return NotFound();
            }

            // Authorization check
            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (inquiry.Member.Email != user?.Email) return Forbid();
            }

            return View(inquiry);
        }

        // GET: Inquiries/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiries/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Message")] Inquiry inquiry)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (member == null) return NotFound("Member not found.");

            inquiry.MemberId = member.MemberId;
            inquiry.InquiryDate = DateOnly.FromDateTime(DateTime.Now);
            inquiry.Status = "New";
            inquiry.GuestEmail = user.Email;

            if (ModelState.IsValid)
            {
                _context.Add(inquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inquiry);
        }

        // GET: Inquiries/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.Include(i => i.Member).FirstOrDefaultAsync(i => i.InquiryId == id);
            if (inquiry == null)
            {
                return NotFound();
            }

            // Security: Only Admins can edit (usually for status updates) 
            // Owners can't really edit content once sent, but let's allow Admin for responses/status
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(inquiry);
        }

        // POST: Inquiries/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InquiryId,Status")] Inquiry inquiryData)
        {
            if (id != inquiryData.InquiryId)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    inquiry.Status = inquiryData.Status;
                    _context.Update(inquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InquiryExists(inquiry.InquiryId))
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
            return View(inquiry);
        }

        // GET: Inquiries/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries
                .Include(i => i.Member)
                .FirstOrDefaultAsync(m => m.InquiryId == id);
            if (inquiry == null)
            {
                return NotFound();
            }

            // Security: Only Admin or Owner can delete
            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (inquiry.Member?.Email != user?.Email) return Forbid();
            }

            return View(inquiry);
        }

        // POST: Inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inquiry = await _context.Inquiries.Include(i => i.Member).FirstOrDefaultAsync(i => i.InquiryId == id);
            if (inquiry == null) return NotFound();

            // Security: Only Admin or Owner can delete
            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (inquiry.Member?.Email != user?.Email) return Forbid();
            }

            _context.Inquiries.Remove(inquiry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InquiryExists(int id)
        {
            return _context.Inquiries.Any(e => e.InquiryId == id);
        }
    }
}
