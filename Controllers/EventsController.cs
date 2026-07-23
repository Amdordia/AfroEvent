using AfroEvent.Hubs;
using AfroEvent.Models;
using AfroEvent.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AfroEvent.Controllers;

public class EventsController : Controller
{
    private readonly EventStore _store = EventStore.Instance;
    private readonly IHubContext<EventHub> _hubContext;

    public EventsController(IHubContext<EventHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public IActionResult Index()
    {
        var events = _store.GetAll();
        return View(events);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new EventCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EventCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var eventItem = _store.Create(new EventItem
        {
            Name = model.Name,
            Description = model.Description,
            ShortDescription = model.ShortDescription,
            Program = model.Program,
            Speakers = model.Speakers,
            Category = model.Category,
            Location = model.Location,
            Logistics = model.Logistics,
            StartDate = model.StartDate,
            Price = model.Price,
            Capacity = model.Capacity,
            Latitude = model.Latitude,
            Longitude = model.Longitude
        });

        return RedirectToAction(nameof(Details), new { id = eventItem.Id });
    }

    public IActionResult Details(int id)
    {
        var eventItem = _store.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int id)
    {
        var eventItem = _store.ReservePlace(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        await _hubContext.Clients.Group(id.ToString()).SendAsync("ReceivePlacesUpdate", eventItem.Id, eventItem.AvailablePlaces, eventItem.Capacity);

        TempData["Message"] = "Votre réservation a été prise en compte.";
        return RedirectToAction("SInscrire", "Participant", new { nom = eventItem.Name });
    }
}
