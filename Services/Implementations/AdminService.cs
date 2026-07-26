using System;
using System.Collections.Generic;
using System.Linq;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private static readonly List<OrganisateurItemViewModel> _organisateurs = new()
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

        private static readonly List<ActiviteRecenteViewModel> _activites = new()
        {
            new ActiviteRecenteViewModel { Description = "Inscriptions ouvertes pour 'Grand Nuit du Mandingue'", DateHeure = DateTime.Now.AddMinutes(-25), Type = "Événement", TypeBadgeClass = "bg-warning text-dark" },
            new ActiviteRecenteViewModel { Description = "Nouvelle demande de compte organisateur : Innov'Sahel", DateHeure = DateTime.Now.AddHours(-2), Type = "Organisateur", TypeBadgeClass = "bg-dark text-warning" },
            new ActiviteRecenteViewModel { Description = "Retrait simulé effectué : 1 200 000 FCFA par Mali Tech Hub", DateHeure = DateTime.Now.AddHours(-5), Type = "Finance", TypeBadgeClass = "bg-success text-white" },
            new ActiviteRecenteViewModel { Description = "Alerte capacité atteinte (90%) : Concert CICB", DateHeure = DateTime.Now.AddDays(-1), Type = "Alerte", TypeBadgeClass = "bg-danger text-white" }
        };

        public AdminDashboardViewModel GetDashboardData()
        {
            lock (_organisateurs)
            {
                var enAttente = _organisateurs.Where(o => o.Statut == "En attente").ToList();

                return new AdminDashboardViewModel
                {
                    TotalOrganisateurs = 48,
                    TotalEvenements = 124,
                    TotalBilletsVendus = 3450,
                    TotalRevenusSimules = 28750000,
                    OrganisateursEnAttenteCount = enAttente.Count,
                    OrganisateursEnAttente = enAttente,
                    ActivitesRecentes = _activites.ToList(),
                    RevenusParCategorie = new List<RevenuParCategorieViewModel>
                    {
                        new RevenuParCategorieViewModel { Categorie = "Concerts & Spectacles", Montant = 14500000, NombreBillets = 1800, Pourcentage = 50.4 },
                        new RevenuParCategorieViewModel { Categorie = "Hackathons & Tech", Montant = 6800000, NombreBillets = 650, Pourcentage = 23.6 },
                        new RevenuParCategorieViewModel { Categorie = "Bootcamps & Formations", Montant = 5250000, NombreBillets = 320, Pourcentage = 18.3 },
                        new RevenuParCategorieViewModel { Categorie = "Conférences & Forums", Montant = 2200000, NombreBillets = 680, Pourcentage = 7.7 }
                    }
                };
            }
        }

        public bool ApproveOrganizer(Guid id)
        {
            lock (_organisateurs)
            {
                var org = _organisateurs.FirstOrDefault(o => o.Id == id);
                if (org != null)
                {
                    org.Statut = "Approuvé";
                    _activites.Insert(0, new ActiviteRecenteViewModel
                    {
                        Description = $"Organisateur approuvé : {org.NomOrganisation} ({org.NomComplet})",
                        DateHeure = DateTime.Now,
                        Type = "Organisateur",
                        TypeBadgeClass = "bg-success text-white"
                    });
                    return true;
                }
                return false;
            }
        }

        public bool RejectOrganizer(Guid id)
        {
            lock (_organisateurs)
            {
                var org = _organisateurs.FirstOrDefault(o => o.Id == id);
                if (org != null)
                {
                    org.Statut = "Rejeté";
                    _activites.Insert(0, new ActiviteRecenteViewModel
                    {
                        Description = $"Demande d'organisateur rejetée : {org.NomOrganisation}",
                        DateHeure = DateTime.Now,
                        Type = "Organisateur",
                        TypeBadgeClass = "bg-danger text-white"
                    });
                    return true;
                }
                return false;
            }
        }
    }
}
