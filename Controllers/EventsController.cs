using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers;

public class EventsController : Controller
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // GET: /Events
    public IActionResult Index()
    {
        var events = _eventService.GetAllEvents();
        return View(events);
    }

    // GET: /Events/Details/5
    public IActionResult Details(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
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
            AgendaItems = new System.Collections.Generic.List<AgendaItemViewModel>
            {
                new AgendaItemViewModel { Id = 1, Title = "Accueil & Inscriptions", StartTime = DateTime.Now.AddDays(14).Date.AddHours(9), EndTime = DateTime.Now.AddDays(14).Date.AddHours(10) },
                new AgendaItemViewModel { Id = 2, Title = "Keynote d'ouverture", StartTime = DateTime.Now.AddDays(14).Date.AddHours(10), EndTime = DateTime.Now.AddDays(14).Date.AddHours(12) }
            },
            Speakers = new System.Collections.Generic.List<SpeakerViewModel>
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

        _eventService.CreateEvent(model);
        TempData["SuccessMessage"] = $"L'événement '{model.Title}' a été créé avec succès !";
        return RedirectToAction("Dashboard", "Organizer");
    }

    // GET: /Events/Edit/5
    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
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

        _eventService.UpdateEvent(id, model);
        TempData["SuccessMessage"] = $"L'événement '{model.Title}' a été mis à jour avec succès !";
        return RedirectToAction("Events", "Organizer");
    }

    // POST: /Events/Reserve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
        await _eventService.ReservePlaceAsync(id);

        TempData["Message"] = "Votre réservation a été prise en compte.";
        return RedirectToAction("SInscrire", "Participant", new { nom = eventModel.Title });
    }
}
