using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;

namespace AfroEvent.Controllers;

/// <summary>
/// Page d'accueil publique. Affiche les prochains événements depuis la vraie BDD.
/// </summary>
public class HomeController : Controller
{
    private readonly IEventService _eventService;

    public HomeController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public IActionResult Index()
    {
        // Prochains événements (max 6) depuis la BDD, pas depuis un singleton in-memory
        var events = _eventService.GetUpcomingEvents(6);
        ViewBag.EventList = events;
        return View();
    }

    public IActionResult Contact()
    {
        ViewBag.Email     = "support@afroevent.ml";
        ViewData["Telephone"] = "+223 00 00 00 00";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
