namespace AfroEvent.Models;

public class EventItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public string Speakers { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Logistics { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int AvailablePlaces { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
