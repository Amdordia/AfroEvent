using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AfroEvent.Controllers;

public class EventsController : Controller
{
	public IActionResult Index()
	{
		var events = new List<string>
		{
			"Hackathon Bamako",
			"Concert CICB",
			"Bootcamp Tech",
			"Salon de l'Innovation"
		};

		ViewBag.EventList = events;
		return View();
	}
}
