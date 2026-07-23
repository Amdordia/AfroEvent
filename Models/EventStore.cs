namespace AfroEvent.Models;

public class EventStore
{
    private static readonly EventStore _instance = new();
    private readonly List<EventItem> _events = new();

    private EventStore()
    {
        Seed();
    }

    public static EventStore Instance => _instance;

    public IReadOnlyList<EventItem> GetAll() => _events.OrderBy(e => e.StartDate).ToList();

    public EventItem? GetById(int id) => _events.FirstOrDefault(e => e.Id == id);

    public EventItem? GetByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return _events.FirstOrDefault();
        }

        return _events.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public EventItem Create(EventItem eventItem)
    {
        eventItem.Id = _events.Count == 0 ? 1 : _events.Max(e => e.Id) + 1;
        eventItem.AvailablePlaces = eventItem.Capacity;
        _events.Add(eventItem);
        return eventItem;
    }

    public EventItem? ReservePlace(int id)
    {
        var eventItem = _events.FirstOrDefault(e => e.Id == id);
        if (eventItem is null || eventItem.AvailablePlaces <= 0)
        {
            return eventItem;
        }

        eventItem.AvailablePlaces--;
        return eventItem;
    }

    private void Seed()
    {
        Create(new EventItem
        {
            Name = "Hackathon Bamako",
            Description = "Un weekend intensif pour créer des solutions digitales autour de l'innovation et de l'entrepreneuriat en Afrique.",
            ShortDescription = "Innovation et code en plein cœur de Bamako.",
            Program = "9h00 - Accueil et pitchs\n10h30 - Workshops IA et design\n14h00 - Sessions de prototypage\n18h00 - Demo night",
            Speakers = "Awa Diakité, Boubacar Sissoko, Mariam Diallo",
            Category = "Technologie",
            Location = "CICB, Bamako",
            Logistics = "Accès gratuit au parking, wifi disponible, restauration sur place",
            StartDate = DateTime.Today.AddDays(7),
            Price = 2500m,
            Capacity = 120,
            AvailablePlaces = 120,
            Latitude = 12.6392,
            Longitude = -8.0029
        });

        Create(new EventItem
        {
            Name = "Concert CICB",
            Description = "Une soirée musicale mêlant rythmes traditionnels et artistes urbains pour célébrer la culture locale.",
            ShortDescription = "Soirée culturelle et musicale.",
            Program = "20h00 - Ouverture\n21h00 - Performance live\n22h30 - DJ set",
            Speakers = "Kadiatou, Yoro, Les Perles du Sahel",
            Category = "Culture",
            Location = "Esplanade du CICB, Bamako",
            Logistics = "Entrée avec billet électronique, consigne disponible, accès PMR",
            StartDate = DateTime.Today.AddDays(12),
            Price = 1500m,
            Capacity = 300,
            AvailablePlaces = 300,
            Latitude = 12.6392,
            Longitude = -8.0029
        });
    }
}
