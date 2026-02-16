using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models;

namespace TicketHub.Controllers
{
    public class InquiriesController : Controller
    {
        private readonly AppDbContext _context;

        public InquiriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Inquiries
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Inquiries.Include(i => i.Member);
            return View(await appDbContext.ToListAsync());
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

            return View(inquiry);
        }

        // GET: Inquiries/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId");
            return View();
        }

        // POST: Inquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InquiryId,GuestEmail,Message,InquiryDate,Status,MemberId")] Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", inquiry.MemberId);
            return View(inquiry);
        }

        // GET: Inquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", inquiry.MemberId);
            return View(inquiry);
        }

        // POST: Inquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InquiryId,GuestEmail,Message,InquiryDate,Status,MemberId")] Inquiry inquiry)
        {
            if (id != inquiry.InquiryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "MemberId", inquiry.MemberId);
            return View(inquiry);
        }

        // GET: Inquiries/Delete/5
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

            return View(inquiry);
        }

        // POST: Inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                _context.Inquiries.Remove(inquiry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InquiryExists(int id)
        {
            return _context.Inquiries.Any(e => e.InquiryId == id);
        }
    }
}
