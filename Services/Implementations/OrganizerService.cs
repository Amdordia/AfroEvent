using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    public class OrganizerService : IOrganizerService
    {
        private readonly AfroEventDbContext _context;

        public OrganizerService(AfroEventDbContext context)
        {
            _context = context;
        }

        public OrganizerDashboardViewModel GetDashboardData()
        {
            var recentEvents = GetOrganizerEvents();
            
            // Calculate actual total revenue from paid tickets
            decimal totalRevenue = 0;
            var paidTickets = _context.Tickets.Include(t => t.Event).Where(t => t.IsPaid).ToList();
            foreach (var ticket in paidTickets)
            {
                if (ticket.Event != null)
                {
                    totalRevenue += ticket.Event.Price;
                }
            }

            return new OrganizerDashboardViewModel
            {
                OrganizerName = "ANNOORA Tech Hub",
                TotalRevenueFcfa = totalRevenue,
                TotalEvents = recentEvents.Count,
                TotalRegistrations = recentEvents.Sum(e => e.RegisteredCount),
                TotalCheckIns = recentEvents.Sum(e => e.CheckedInCount),
                RecentEvents = recentEvents
            };
        }

        public List<OrganizerEventSummaryViewModel> GetOrganizerEvents()
        {
            var events = _context.Events
                .Include(e => e.Category)
                .ToList();

            var tickets = _context.Tickets.ToList();

            return events.Select(e => new OrganizerEventSummaryViewModel
            {
                Id = e.Id,
                Title = e.Title,
                CategoryName = e.Category?.Name ?? "Général",
                StartDate = e.StartDate,
                LocationAddress = e.LocationAddress,
                MaxCapacity = e.MaxCapacity,
                RegisteredCount = tickets.Count(t => t.EventId == e.Id),
                CheckedInCount = tickets.Count(t => t.EventId == e.Id && t.IsPresent),
                TicketPrice = e.Price,
                Status = e.StartDate > DateTime.Now ? "Publié" : "Terminé"
            }).ToList();
        }

        public List<AttendeeViewModel> GetAttendeesForEvent(Guid eventId)
        {
            var tickets = _context.Tickets
                .Where(t => t.EventId == eventId)
                .ToList();

            // Retrieve corresponding users if possible, or build default views
            var attendees = new List<AttendeeViewModel>();
            foreach (var ticket in tickets)
            {
                // Retrieve participant details from AppUsers table if needed
                var user = _context.Users.FirstOrDefault(u => u.Id == ticket.ParticipantId);
                var fullName = user != null ? $"{user.UserName}" : "Participant";
                var email = user != null ? user.Email ?? string.Empty : "email@afroevent.com";
                var phone = user != null ? user.PhoneNumber ?? string.Empty : string.Empty;

                attendees.Add(new AttendeeViewModel
                {
                    TicketId = ticket.Id,
                    ParticipantName = fullName,
                    Email = email,
                    PhoneNumber = phone,
                    RegistrationDate = ticket.PurchaseDate,
                    IsPaid = ticket.IsPaid,
                    IsPresent = ticket.IsPresent,
                    ScanDate = ticket.ScanDate,
                    QrCodeHash = ticket.QrCodeHash
                });
            }

            return attendees;
        }

        public bool CheckInAttendee(Guid ticketId)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket != null && !ticket.IsPresent)
            {
                ticket.IsPresent = true;
                ticket.ScanDate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
