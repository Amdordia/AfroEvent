using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers;

public class EventsController : Controller
{
    // GET: /Events
    public IActionResult Index()
    {
        var events = GetMockEventFormViewModels();
        return View(events);
    }

    // GET: /Events/Details/5
    public IActionResult Details(Guid id)
    {
        var eventModel = GetMockEventFormViewModels().Find(e => e.Id == id) ?? GetMockEventFormViewModels()[0];
        return View(eventModel);
    }

    // GET: /Events/Create
    [HttpGet]
    public IActionResult Create()
    {
        var model = new EventFormViewModel
        {
            StartDate = DateTime.Now.AddDays(14).Date.AddHours(9),
            EndDate = DateTime.Now.AddDays(14).Date.AddHours(18),
            AgendaItems = new List<AgendaItemViewModel>
            {
                new AgendaItemViewModel { Id = 1, Title = "Accueil & Inscriptions", StartTime = DateTime.Now.AddDays(14).Date.AddHours(9), EndTime = DateTime.Now.AddDays(14).Date.AddHours(10) },
                new AgendaItemViewModel { Id = 2, Title = "Keynote d'ouverture", StartTime = DateTime.Now.AddDays(14).Date.AddHours(10), EndTime = DateTime.Now.AddDays(14).Date.AddHours(12) }
            },
            Speakers = new List<SpeakerViewModel>
            {
                new SpeakerViewModel { Id = 1, FullName = "Dr. Seydou Keita", Biography = "Expert IA & Solutions Cloud", ProfileImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80" }
            }
        };

        return View(model);
    }

    // POST: /Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EventFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData["SuccessMessage"] = $"L'événement '{model.Title}' a été créé avec succès !";
        return RedirectToAction("Dashboard", "Organizer");
    }

    // GET: /Events/Edit/5
    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var eventModel = GetMockEventFormViewModels().Find(e => e.Id == id) ?? GetMockEventFormViewModels()[0];
        return View(eventModel);
    }

    // POST: /Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, EventFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData["SuccessMessage"] = $"L'événement '{model.Title}' a été mis à jour avec succès !";
        return RedirectToAction("Events", "Organizer");
    }

    private List<EventFormViewModel> GetMockEventFormViewModels()
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
