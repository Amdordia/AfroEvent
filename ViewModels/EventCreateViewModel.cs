using System.ComponentModel.DataAnnotations;

namespace AfroEvent.ViewModels;

public class EventCreateViewModel
{
    [Required(ErrorMessage = "Le nom est requis")]
    [Display(Name = "Nom de l'événement")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est requise")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Résumé court")]
    public string ShortDescription { get; set; } = string.Empty;

    [Display(Name = "Programme")]
    public string Program { get; set; } = string.Empty;

    [Display(Name = "Intervenants")]
    public string Speakers { get; set; } = string.Empty;

    [Display(Name = "Catégorie")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Lieu")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Logistique")]
    public string Logistics { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date de début")]
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(7);

    [Required]
    [Display(Name = "Prix du billet")]
    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Nombre de places")]
    [Range(1, 10000)]
    public int Capacity { get; set; } = 100;

    [Display(Name = "Latitude")]
    public double Latitude { get; set; } = 12.6392;

    [Display(Name = "Longitude")]
    public double Longitude { get; set; } = -8.0029;
}
