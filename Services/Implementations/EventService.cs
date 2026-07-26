using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AfroEvent.Hubs;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly List<EventFormViewModel> _events;

        public EventService(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
            _events = SeedInitialEvents();
        }

        public List<EventFormViewModel> GetAllEvents()
        {
            lock (_events)
            {
                return _events.ToList();
            }
        }

        public EventFormViewModel GetEventById(Guid id)
        {
            lock (_events)
            {
                var evt = _events.FirstOrDefault(e => e.Id == id);
                return evt ?? _events.FirstOrDefault() ?? new EventFormViewModel { Title = "Événement introuvable" };
            }
        }

        public void CreateEvent(EventFormViewModel model)
        {
            lock (_events)
            {
                if (model.Id == Guid.Empty)
                {
                    model.Id = Guid.NewGuid();
                }

                if (string.IsNullOrEmpty(model.CategoryName))
                {
                    model.CategoryName = model.CategoryId switch
                    {
                        1 => "Hackathon",
                        2 => "Bootcamp",
                        3 => "Conférence",
                        _ => "Général"
                    };
                }

                _events.Insert(0, model);
            }
        }

        public void UpdateEvent(Guid id, EventFormViewModel model)
        {
            lock (_events)
            {
                var existingIndex = _events.FindIndex(e => e.Id == id);
                if (existingIndex >= 0)
                {
                    model.Id = id;
                    _events[existingIndex] = model;
                }
            }
        }

        public async Task ReservePlaceAsync(Guid id)
        {
            EventFormViewModel? evt;
            lock (_events)
            {
                evt = _events.FirstOrDefault(e => e.Id == id);
            }

            if (evt != null)
            {
                // Emit SignalR Realtime notification
                await _hubContext.Clients.All.SendAsync("ReceivePlacesUpdate", id.ToString(), 14, evt.MaxCapacity);
            }
        }

        private static List<EventFormViewModel> SeedInitialEvents()
        {
            return new List<EventFormViewModel>
            {
                new EventFormViewModel
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Hackathon Bamako 2026",
                    Description = "Le plus grand rassemblement de développeurs, designers et créateurs de solutions numériques au Mali. Relevez des défis technologiques sur 48 heures chronométrées avec mentoring intensif et prix d'innovation !",
                    StartDate = DateTime.Now.AddDays(3).Date.AddHours(8),
                    EndDate = DateTime.Now.AddDays(5).Date.AddHours(20),
                    LocationAddress = "Centre International de Conférences de Bamako (CICB), Quartier du Fleuve",
                    Latitude = 12.6342,
                    Longitude = -7.9989,
                    Price = 10000,
                    MaxCapacity = 200,
                    CategoryId = 1,
                    CategoryName = "Hackathon",
                    CoverImageUrl = "https://images.unsplash.com/photo-1515187029135-18ee286d815b?auto=format&fit=crop&w=1200&q=80",
                    AgendaItems = new List<AgendaItemViewModel>
                    {
                        new AgendaItemViewModel { Id = 1, Title = "Cérémonie d'ouverture & Lancement des sujets", StartTime = DateTime.Now.AddDays(3).Date.AddHours(9), EndTime = DateTime.Now.AddDays(3).Date.AddHours(11) },
                        new AgendaItemViewModel { Id = 2, Title = "Session de Mentoring & Pitch Intermediate", StartTime = DateTime.Now.AddDays(4).Date.AddHours(14), EndTime = DateTime.Now.AddDays(4).Date.AddHours(17) },
                        new AgendaItemViewModel { Id = 3, Title = "Grand Jury & Remise des Prix", StartTime = DateTime.Now.AddDays(5).Date.AddHours(16), EndTime = DateTime.Now.AddDays(5).Date.AddHours(19) }
                    },
                    Speakers = new List<SpeakerViewModel>
                    {
                        new SpeakerViewModel { Id = 1, FullName = "Aminata Koné", Biography = "Lead Software Architect chez ANNOORA Tech", LinkedInUrl = "https://linkedin.com", ProfileImageUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=300&q=80" },
                        new SpeakerViewModel { Id = 2, FullName = "Ibrahim Touré", Biography = "Fondateur de Mali Innovation Hub", LinkedInUrl = "https://linkedin.com", ProfileImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=300&q=80" }
                    }
                },
                new EventFormViewModel
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "Bootcamp Full-Stack ASP.NET Core & EF Core",
                    Description = "Une formation intensive de 5 jours pour maîtriser le développement web moderne en C#, ASP.NET Core MVC, Entity Framework Core, SQL Server et architectures N-Tiers.",
                    StartDate = DateTime.Now.AddDays(14).Date.AddHours(9),
                    EndDate = DateTime.Now.AddDays(19).Date.AddHours(17),
                    LocationAddress = "Espace Baïta Innovation Hub, Hamdallaye ACI 2000, Bamako",
                    Latitude = 12.6285,
                    Longitude = -8.0210,
                    Price = 25000,
                    MaxCapacity = 50,
                    CategoryId = 2,
                    CategoryName = "Bootcamp",
                    CoverImageUrl = "https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=1200&q=80",
                    AgendaItems = new List<AgendaItemViewModel>
                    {
                        new AgendaItemViewModel { Id = 1, Title = "Architecture ASP.NET Core & Dependency Injection", StartTime = DateTime.Now.AddDays(14).Date.AddHours(9), EndTime = DateTime.Now.AddDays(14).Date.AddHours(12) },
                        new AgendaItemViewModel { Id = 2, Title = "EF Core, Migrations & Repository Pattern", StartTime = DateTime.Now.AddDays(15).Date.AddHours(9), EndTime = DateTime.Now.AddDays(15).Date.AddHours(17) }
                    },
                    Speakers = new List<SpeakerViewModel>
                    {
                        new SpeakerViewModel { Id = 1, FullName = "PERFECT_Dev", Biography = "Architecte Code & Ingénieur Logiciel ANNOORA", ProfileImageUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=300&q=80" }
                    }
                }
            };
        }
    }
}
