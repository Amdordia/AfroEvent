using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;

namespace AfroEvent.Controllers;

public class HomeController : Controller
{
    private readonly EventStore _store = EventStore.Instance;

    public IActionResult Index()
    {
        var events = _store.GetAll();
        ViewBag.EventList = events;
        return View();
    }

    public IActionResult Contact()
    {
        ViewBag.Email = "support@afroevent.ml";
        ViewData["Telephone"] = "+223 00 00 00 00";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Details(string nom)
    {
        ViewBag.EventName = nom ?? "Événement";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
