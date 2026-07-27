using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;

namespace AfroEvent.Controllers;

/// <summary>
/// Espace organisateur. Filtrage automatique des données par l'ID de l'organisateur connecté.
/// </summary>
[Authorize(Roles = "Admin,Organisateur")]
public class OrganizerController : Controller
{
    private readonly IOrganizerService _organizerService;
    private readonly IEventService     _eventService;
    private readonly UserManager<AppUser> _userManager;

    public OrganizerController(
        IOrganizerService organizerService,
        IEventService eventService,
        UserManager<AppUser> userManager)
    {
        _organizerService = organizerService;
        _eventService     = eventService;
        _userManager      = userManager;
    }

    // GET: /Organizer/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var user          = await _userManager.GetUserAsync(User);
        var organizerId   = user?.Id ?? string.Empty;
        var organizerName = user?.OrganizationName is { Length: > 0 } orgName
                            ? orgName
                            : user?.FullName ?? "Organisateur";

        var model = _organizerService.GetDashboardData(organizerId, organizerName);
        return View(model);
    }

    // GET: /Organizer/Events
    public async Task<IActionResult> Events()
    {
        var user        = await _userManager.GetUserAsync(User);
        var organizerId = user?.Id ?? string.Empty;
        var model       = _organizerService.GetOrganizerEvents(organizerId);
        return View(model);
    }

    // GET: /Organizer/Attendees/5
    public IActionResult Attendees(Guid id)
    {
        var evt = _eventService.GetEventById(id);
        ViewBag.EventTitle = evt?.Title ?? "Événement";
        ViewBag.EventId    = id;

        var attendees = _organizerService.GetAttendeesForEvent(id);
        return View(attendees);
    }

    // POST: /Organizer/CheckIn
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckIn(Guid ticketId, Guid eventId)
    {
        var success = _organizerService.CheckInAttendee(ticketId);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? "Participant validé avec succès (Présence confirmée)."
            : "Ce billet est déjà scanné ou invalide.";

        return RedirectToAction(nameof(Attendees), new { id = eventId });
    }
}
