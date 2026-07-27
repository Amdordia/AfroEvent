using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Models;

namespace AfroEvent.__useDefaultLayoutControllers
{
    public class TicketsController : Controller
    {
        private readonly AfroEventDbContext _context;

        public TicketsController(AfroEventDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var afroEventDbContext = _context.Tickets.Include(t => t.Event);
            return View(await afroEventDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketEntity = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketEntity == null)
            {
                return NotFound();
            }

            return View(ticketEntity);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QrCodeHash,IsPaid,IsPresent,ScanDate,PurchaseDate,EventId,ParticipantId")] TicketEntity ticketEntity)
        {
            if (ModelState.IsValid)
            {
                ticketEntity.Id = Guid.NewGuid();
                _context.Add(ticketEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", ticketEntity.EventId);
            return View(ticketEntity);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketEntity = await _context.Tickets.FindAsync(id);
            if (ticketEntity == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", ticketEntity.EventId);
            return View(ticketEntity);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,QrCodeHash,IsPaid,IsPresent,ScanDate,PurchaseDate,EventId,ParticipantId")] TicketEntity ticketEntity)
        {
            if (id != ticketEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketEntityExists(ticketEntity.Id))
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
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", ticketEntity.EventId);
            return View(ticketEntity);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketEntity = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketEntity == null)
            {
                return NotFound();
            }

            return View(ticketEntity);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ticketEntity = await _context.Tickets.FindAsync(id);
            if (ticketEntity != null)
            {
                _context.Tickets.Remove(ticketEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketEntityExists(Guid id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
