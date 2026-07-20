using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;

namespace AfroEvent.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
