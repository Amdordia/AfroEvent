using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AfroEvent.Services.Interfaces;

namespace AfroEvent.Controllers;

[Authorize(Roles = "Admin, Organisateur")]

public class OrganizerController : Controller
{
    private readonly IOrganizerService _organizerService;

    public OrganizerController(IOrganizerService organizerService)
    {
        _organizerService = organizerService;
    }

    // GET: /Organizer/Dashboard
    public IActionResult Dashboard()
    {
        var model = _organizerService.GetDashboardData();
        return View(model);
    }

    // GET: /Organizer/Events
    public IActionResult Events()
    {
        var model = _organizerService.GetOrganizerEvents();
        return View(model);
    }

    // GET: /Organizer/Attendees/5
    public IActionResult Attendees(Guid id)
    {
        ViewBag.EventTitle = "Hackathon Bamako 2026";
        ViewBag.EventId = id;

        var attendees = _organizerService.GetAttendeesForEvent(id);
        return View(attendees);
    }

    // POST: /Organizer/CheckIn/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckIn(Guid ticketId, Guid eventId)
    {
        var success = _organizerService.CheckInAttendee(ticketId);
        if (success)
        {
            TempData["SuccessMessage"] = "Participant validé avec succès (Présence confirmée).";
        }
        else
        {
            TempData["ErrorMessage"] = "Ce billet est déjà scanné ou invalide.";
        }

        return RedirectToAction(nameof(Attendees), new { id = eventId });
    }
}
