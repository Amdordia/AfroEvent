using System;
using System.Collections.Generic;
using System.Linq;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    public class OrganizerService : IOrganizerService
    {
        private readonly List<AttendeeViewModel> _attendees;

        public OrganizerService()
        {
            _attendees = SeedMockAttendees();
        }

        public OrganizerDashboardViewModel GetDashboardData()
        {
            var recentEvents = GetOrganizerEvents();

            return new OrganizerDashboardViewModel
            {
                OrganizerName = "ANNOORA Tech Hub",
                TotalRevenueFcfa = 3450000,
                TotalEvents = recentEvents.Count,
                TotalRegistrations = recentEvents.Sum(e => e.RegisteredCount),
                TotalCheckIns = recentEvents.Sum(e => e.CheckedInCount),
                RecentEvents = recentEvents
            };
        }

        public List<OrganizerEventSummaryViewModel> GetOrganizerEvents()
        {
            return new List<OrganizerEventSummaryViewModel>
            {
                new OrganizerEventSummaryViewModel
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Hackathon Bamako 2026",
                    CategoryName = "Hackathon",
                    StartDate = DateTime.Now.AddDays(3),
                    LocationAddress = "Centre International de Conférences de Bamako (CICB)",
                    MaxCapacity = 200,
                    RegisteredCount = 185,
                    CheckedInCount = 160,
                    TicketPrice = 10000,
                    Status = "Publié"
                },
                new OrganizerEventSummaryViewModel
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "Bootcamp Full-Stack ASP.NET Core",
                    CategoryName = "Bootcamp",
                    StartDate = DateTime.Now.AddDays(14),
                    LocationAddress = "Espace Baïta Innovation Hub, Hamdallaye ACI 2000",
                    MaxCapacity = 50,
                    RegisteredCount = 48,
                    CheckedInCount = 0,
                    TicketPrice = 25000,
                    Status = "Publié"
                },
                new OrganizerEventSummaryViewModel
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Title = "Forum de la Tech & IA en Afrique",
                    CategoryName = "Conférence",
                    StartDate = DateTime.Now.AddDays(30),
                    LocationAddress = "Hôtel de l'Amitié, Bamako",
                    MaxCapacity = 500,
                    RegisteredCount = 310,
                    CheckedInCount = 0,
                    TicketPrice = 5000,
                    Status = "Publié"
                },
                new OrganizerEventSummaryViewModel
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Title = "Atelier Design System & Diogner UI",
                    CategoryName = "Workshop",
                    StartDate = DateTime.Now.AddDays(-10),
                    LocationAddress = "Salle de Conférence ANNOORA Studio",
                    MaxCapacity = 40,
                    RegisteredCount = 40,
                    CheckedInCount = 38,
                    TicketPrice = 0,
                    Status = "Terminé"
                },
                new OrganizerEventSummaryViewModel
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Title = "Concert Afrobeats & Tech Night",
                    CategoryName = "Concert",
                    StartDate = DateTime.Now.AddDays(45),
                    LocationAddress = "Palais de la Culture Amadou Hampâté Ba",
                    MaxCapacity = 1000,
                    RegisteredCount = 37,
                    CheckedInCount = 0,
                    TicketPrice = 15000,
                    Status = "Brouillon"
                }
            };
        }

        public List<AttendeeViewModel> GetAttendeesForEvent(Guid eventId)
        {
            lock (_attendees)
            {
                return _attendees.ToList();
            }
        }

        public bool CheckInAttendee(Guid ticketId)
        {
            lock (_attendees)
            {
                var attendee = _attendees.FirstOrDefault(a => a.TicketId == ticketId);
                if (attendee != null && !attendee.IsPresent)
                {
                    attendee.IsPresent = true;
                    attendee.ScanDate = DateTime.Now;
                    return true;
                }
                return false;
            }
        }

        private static List<AttendeeViewModel> SeedMockAttendees()
        {
            return new List<AttendeeViewModel>
            {
                new AttendeeViewModel
                {
                    TicketId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ParticipantName = "Aminata Diallo",
                    Email = "aminata.diallo@afroevent.com",
                    PhoneNumber = "+223 76 12 34 56",
                    RegistrationDate = DateTime.Now.AddDays(-5),
                    IsPaid = true,
                    IsPresent = true,
                    ScanDate = DateTime.Now.AddHours(-2),
                    QrCodeHash = "AFRO-TKT-89123-X"
                },
                new AttendeeViewModel
                {
                    TicketId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    ParticipantName = "Moussa Traoré",
                    Email = "moussa.traore@afroevent.com",
                    PhoneNumber = "+223 65 98 76 54",
                    RegistrationDate = DateTime.Now.AddDays(-4),
                    IsPaid = true,
                    IsPresent = false,
                    ScanDate = null,
                    QrCodeHash = "AFRO-TKT-44109-Y"
                },
                new AttendeeViewModel
                {
                    TicketId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    ParticipantName = "Fanta Coulibaly",
                    Email = "fanta.coulibaly@afroevent.com",
                    PhoneNumber = "+223 90 11 22 33",
                    RegistrationDate = DateTime.Now.AddDays(-3),
                    IsPaid = true,
                    IsPresent = true,
                    ScanDate = DateTime.Now.AddHours(-1),
                    QrCodeHash = "AFRO-TKT-99120-Z"
                },
                new AttendeeViewModel
                {
                    TicketId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    ParticipantName = "Oumar Sissoko",
                    Email = "oumar.sissoko@afroevent.com",
                    PhoneNumber = "+223 70 88 99 00",
                    RegistrationDate = DateTime.Now.AddDays(-1),
                    IsPaid = false,
                    IsPresent = false,
                    ScanDate = null,
                    QrCodeHash = "AFRO-TKT-12001-A"
                }
            };
        }
    }
}
