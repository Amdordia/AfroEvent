using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers;

/// <summary>
/// Gestion des événements (CRUD). Lecture publique, création/modification réservées aux organisateurs.
/// </summary>
public class EventsController : Controller
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // GET: /Events — Accessible à tous
    public IActionResult Index()
    {
        var events = _eventService.GetAllEvents();
        return View(events);
    }

    // GET: /Events/Details/5 — Accessible à tous
    public IActionResult Details(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
        if (eventModel == null)
        {
            TempData["ErrorMessage"] = "Événement introuvable.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AvailablePlaces = _eventService.GetAvailablePlaces(id);
        return View(eventModel);
    }

    // GET: /Events/Create — Organisateur uniquement
    [HttpGet]
    [Authorize(Roles = "Organisateur")]
    public IActionResult Create()
    {
        var model = new EventFormViewModel
        {
            StartDate = DateTime.Now.AddDays(14).Date.AddHours(9),
            EndDate   = DateTime.Now.AddDays(14).Date.AddHours(18),
        };
        return View(model);
    }

    // POST: /Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organisateur")]
    public IActionResult Create(EventFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        _eventService.CreateEvent(model, organizerId);
        TempData["SuccessMessage"] = $"L'événement \"{model.Title}\" a été créé avec succès !";
        return RedirectToAction("Dashboard", "Organizer");
    }

    // GET: /Events/Edit/5
    [HttpGet]
    [Authorize(Roles = "Organisateur")]
    public IActionResult Edit(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
        if (eventModel == null)
        {
            TempData["ErrorMessage"] = "Événement introuvable.";
            return RedirectToAction(nameof(Index));
        }
        return View(eventModel);
    }

    // POST: /Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organisateur")]
    public IActionResult Edit(Guid id, EventFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _eventService.UpdateEvent(id, model);
        TempData["SuccessMessage"] = $"L'événement \"{model.Title}\" a été mis à jour.";
        return RedirectToAction("Events", "Organizer");
    }

    // POST: /Events/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organisateur")]
    public IActionResult Delete(Guid id)
    {
        var evt = _eventService.GetEventById(id);
        _eventService.DeleteEvent(id);
        TempData["SuccessMessage"] = evt != null
            ? $"L'événement \"{evt.Title}\" a été supprimé."
            : "Événement supprimé.";
        return RedirectToAction("Events", "Organizer");
    }

    // POST: /Events/Reserve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(Guid id)
    {
        var eventModel = _eventService.GetEventById(id);
        if (eventModel == null)
        {
            TempData["ErrorMessage"] = "Événement introuvable.";
            return RedirectToAction(nameof(Index));
        }

        await _eventService.ReservePlaceAsync(id);
        return RedirectToAction("SInscrire", "Participant", new { nom = eventModel.Title });
    }
}
