using System;
using System.Collections.Generic;

namespace AfroEvent.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalOrganisateurs { get; set; }
    public int TotalEvenements { get; set; }
    public int TotalBilletsVendus { get; set; }
    public decimal TotalRevenusSimules { get; set; }

    public int OrganisateursEnAttenteCount { get; set; }
    public int EvenementsEnModerationCount { get; set; }

    public List<OrganisateurItemViewModel> OrganisateursEnAttente { get; set; } = new();
    public List<EvenementModerationViewModel> EvenementsEnModeration { get; set; } = new();
    public List<ActiviteRecenteViewModel> ActivitesRecentes { get; set; } = new();
    public List<RevenuParCategorieViewModel> RevenusParCategorie { get; set; } = new();
}

public class OrganisateurItemViewModel
{
    public Guid Id { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string NomOrganisation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public DateTime DateDemande { get; set; }
    public string Statut { get; set; } = "En attente"; // En attente, Approuvé, Rejeté
}

public class EvenementModerationViewModel
{
    public Guid Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string OrganisateurNom { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public decimal PrixTicket { get; set; }
    public int PlacesReservees { get; set; }
    public int CapaciteMax { get; set; }
    public string Statut { get; set; } = "En attente"; // Publié, En attente, Bloqué
}

public class ActiviteRecenteViewModel
{
    public string Description { get; set; } = string.Empty;
    public DateTime DateHeure { get; set; }
    public string TypeBadgeClass { get; set; } = "bg-warning text-dark";
    public string Type { get; set; } = "Information";
}

public class RevenuParCategorieViewModel
{
    public string Categorie { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public int NombreBillets { get; set; }
    public double Pourcentage { get; set; }
}
