using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers;

public class AdminController : Controller
{
    
    private static readonly List<OrganisateurItemViewModel> _organisateursMock = new()
    {
        new OrganisateurItemViewModel
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            NomComplet = "Amadou Diarra",
            NomOrganisation = "Mali Tech Hub",
            Email = "a.diarra@malitech.ml",
            Telephone = "+223 76 12 34 56",
            DateDemande = DateTime.Now.AddDays(-2),
            Statut = "En attente"
        },
        new OrganisateurItemViewModel
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            NomComplet = "Fatoumata Coulibaly",
            NomOrganisation = "Afro Festival Events",
            Email = "contact@afrofestival.com",
            Telephone = "+223 65 98 76 54",
            DateDemande = DateTime.Now.AddDays(-1),
            Statut = "En attente"
        },
        new OrganisateurItemViewModel
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            NomComplet = "Ibrahima Sissoko",
            NomOrganisation = "Bamako Music Pro",
            Email = "ibrahima@bamakomusic.com",
            Telephone = "+223 70 00 11 22",
            DateDemande = DateTime.Now.AddDays(-5),
            Statut = "Approuvé"
        },
        new OrganisateurItemViewModel
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            NomComplet = "Aïcha Traoré",
            NomOrganisation = "Afro Innov",
            Email = "aicha@innovsahel.org",
            Telephone = "+223 90 44 55 66",
            DateDemande = DateTime.Now.AddHours(-6),
            Statut = "En attente"
        }
    };

    public IActionResult Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalOrganisateurs = 48,
            TotalEvenements = 124,
            TotalBilletsVendus = 3450,
            TotalRevenusSimules = 28750000, // FCFA

            OrganisateursEnAttenteCount = _organisateursMock.Count(o => o.Statut == "En attente"),

            OrganisateursEnAttente = _organisateursMock.Where(o => o.Statut == "En attente").ToList(),
            
            ActivitesRecentes = new List<ActiviteRecenteViewModel>
            {
                new ActiviteRecenteViewModel { Description = "Inscriptions ouvertes pour 'Grand Nuit du Mandingue'", DateHeure = DateTime.Now.AddMinutes(-25), Type = "Événement", TypeBadgeClass = "bg-warning text-dark" },
                new ActiviteRecenteViewModel { Description = "Nouvelle demande de compte organisateur : Innov'Sahel", DateHeure = DateTime.Now.AddHours(-2), Type = "Organisateur", TypeBadgeClass = "bg-dark text-warning" },
                new ActiviteRecenteViewModel { Description = "Retrait simulé effectué : 1 200 000 FCFA par Mali Tech Hub", DateHeure = DateTime.Now.AddHours(-5), Type = "Finance", TypeBadgeClass = "bg-success text-white" },
                new ActiviteRecenteViewModel { Description = "Alerte capacité atteinte (90%) : Concert CICB", DateHeure = DateTime.Now.AddDays(-1), Type = "Alerte", TypeBadgeClass = "bg-danger text-white" }
            },

            RevenusParCategorie = new List<RevenuParCategorieViewModel>
            {
                new RevenuParCategorieViewModel { Categorie = "Concerts & Spectacles", Montant = 14500000, NombreBillets = 1800, Pourcentage = 50.4 },
                new RevenuParCategorieViewModel { Categorie = "Hackathons & Tech", Montant = 6800000, NombreBillets = 650, Pourcentage = 23.6 },
                new RevenuParCategorieViewModel { Categorie = "Bootcamps & Formations", Montant = 5250000, NombreBillets = 320, Pourcentage = 18.3 },
                new RevenuParCategorieViewModel { Categorie = "Conférences & Forums", Montant = 2200000, NombreBillets = 680, Pourcentage = 7.7 }
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApprouverOrganisateur(Guid id)
    {
        var org = _organisateursMock.FirstOrDefault(o => o.Id == id);
        if (org != null)
        {
            org.Statut = "Approuvé";
            TempData["SuccesMessage"] = $"L'organisateur {org.NomOrganisation} ({org.NomComplet}) a été approuvé avec succès.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RejeterOrganisateur(Guid id)
    {
        var org = _organisateursMock.FirstOrDefault(o => o.Id == id);
        if (org != null)
        {
            org.Statut = "Rejeté";
            TempData["WarningMessage"] = $"La demande de {org.NomOrganisation} a été rejetée.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}
