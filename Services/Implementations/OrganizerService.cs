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
    /// <summary>
    /// Service organisateur. Toutes les opérations sont isolées par organizerId.
    /// </summary>
    public class OrganizerService : IOrganizerService
    {
        private readonly AfroEventDbContext _context;

        public OrganizerService(AfroEventDbContext context)
        {
            _context = context;
        }

        public OrganizerDashboardViewModel GetDashboardData(string organizerId, string organizerName)
        {
            var recentEvents = GetOrganizerEvents(organizerId);

            // Calcul du revenu réel de cet organisateur uniquement
            decimal totalRevenue = 0;
            foreach (var evt in recentEvents)
            {
                totalRevenue += evt.RegisteredCount * evt.TicketPrice;
            }

            // Graphe Catégories (Group By CategoryName)
            var categoryGroups = recentEvents
                .GroupBy(e => e.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Sum(e => e.RegisteredCount) })
                .ToList();

            var categoryLabels = categoryGroups.Select(cg => cg.Category).ToList();
            var categoryData = categoryGroups.Select(cg => cg.Count).ToList();

            // Graphe Événements (5 plus récents avec inscrits et revenus)
            var chartEvents = recentEvents.Take(5).Reverse().ToList(); // Inverser pour l'ordre chronologique sur le graphe
            var eventLabels = chartEvents.Select(e => e.Title.Length > 15 ? e.Title[..12] + "..." : e.Title).ToList();
            var eventRegData = chartEvents.Select(e => e.RegisteredCount).ToList();
            var eventRevData = chartEvents.Select(e => e.RegisteredCount * e.TicketPrice).ToList();

            return new OrganizerDashboardViewModel
            {
                OrganizerName      = organizerName,
                TotalRevenueFcfa   = totalRevenue,
                TotalEvents        = recentEvents.Count,
                TotalRegistrations = recentEvents.Sum(e => e.RegisteredCount),
                TotalCheckIns      = recentEvents.Sum(e => e.CheckedInCount),
                RecentEvents       = recentEvents.Take(5).ToList(),
                CategoryLabels     = categoryLabels,
                CategoryData       = categoryData,
                EventLabels        = eventLabels,
                EventRegistrationData = eventRegData,
                EventRevenueData   = eventRevData
            };
        }

        public List<OrganizerEventSummaryViewModel> GetOrganizerEvents(string organizerId)
        {
            var events = _context.Events
                .Include(e => e.Category)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.StartDate)
                .ToList();

            var tickets = _context.Tickets
                .Where(t => events.Select(e => e.Id).Contains(t.EventId))
                .ToList();

            return events.Select(e => new OrganizerEventSummaryViewModel
            {
                Id             = e.Id,
                Title          = e.Title,
                CategoryName   = e.Category?.Name ?? "Général",
                StartDate      = e.StartDate,
                LocationAddress = e.LocationAddress,
                MaxCapacity    = e.MaxCapacity,
                RegisteredCount = tickets.Count(t => t.EventId == e.Id),
                CheckedInCount = tickets.Count(t => t.EventId == e.Id && t.IsPresent),
                TicketPrice    = e.Price,
                Status         = DetermineStatus(e.StartDate, e.EndDate)
            }).ToList();
        }

        public List<AttendeeViewModel> GetAttendeesForEvent(Guid eventId)
        {
            var tickets = _context.Tickets
                .Where(t => t.EventId == eventId)
                .ToList();

            var attendees = new List<AttendeeViewModel>();
            foreach (var ticket in tickets)
            {
                var user     = _context.Users.FirstOrDefault(u => u.Id == ticket.ParticipantId);
                var fullName = user != null ? user.FullName : "Participant";
                var email    = user?.Email ?? string.Empty;
                var phone    = user?.PhoneNumber ?? string.Empty;

                attendees.Add(new AttendeeViewModel
                {
                    TicketId         = ticket.Id,
                    ParticipantName  = fullName,
                    Email            = email,
                    PhoneNumber      = phone,
                    RegistrationDate = ticket.PurchaseDate,
                    IsPaid           = ticket.IsPaid,
                    IsPresent        = ticket.IsPresent,
                    ScanDate         = ticket.ScanDate,
                    QrCodeHash       = ticket.QrCodeHash
                });
            }

            return attendees;
        }

        public bool CheckInAttendee(Guid ticketId)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null || ticket.IsPresent) return false;

            ticket.IsPresent = true;
            ticket.ScanDate  = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }

        private static string DetermineStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            if (now < startDate)  return "Publié";
            if (now <= endDate)   return "En cours";
            return "Terminé";
        }
    }
}
