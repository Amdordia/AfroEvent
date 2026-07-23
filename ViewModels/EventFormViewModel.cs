using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AfroEvent.ViewModels;

public class EventFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Le titre de l'événement est obligatoire.")]
    [Display(Name = "Titre de l'événement")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est obligatoire.")]
    [Display(Name = "Description détaillée")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date de début est obligatoire.")]
    [Display(Name = "Date & Heure de début")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "La date de fin est obligatoire.")]
    [Display(Name = "Date & Heure de fin")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7).AddHours(4);

    [Required(ErrorMessage = "L'adresse du lieu est obligatoire.")]
    [Display(Name = "Adresse / Lieu")]
    public string LocationAddress { get; set; } = string.Empty;

    [Display(Name = "Latitude")]
    public double Latitude { get; set; } = 12.6392; // Coordonnées Bamako

    [Display(Name = "Longitude")]
    public double Longitude { get; set; } = -8.0029;

    [Display(Name = "Prix du billet (FCFA) - 0 si gratuit")]
    [Range(0, 1000000, ErrorMessage = "Le prix doit être positif.")]
    public decimal Price { get; set; } = 0;

    [Required(ErrorMessage = "La capacité maximale est requise.")]
    [Display(Name = "Nombre de places max")]
    [Range(1, 100000, ErrorMessage = "La capacité doit être d'au moins 1 place.")]
    public int MaxCapacity { get; set; } = 100;

    [Display(Name = "URL de l'image de couverture")]
    public string? CoverImageUrl { get; set; }

    [Required(ErrorMessage = "Veuillez sélectionner une catégorie.")]
    [Display(Name = "Catégorie")]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public List<AgendaItemViewModel> AgendaItems { get; set; } = new();
    public List<SpeakerViewModel> Speakers { get; set; } = new();
}

public class AgendaItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class SpeakerViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? LinkedInUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
}
