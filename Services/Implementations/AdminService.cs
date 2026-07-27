using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    /// <summary>
    /// Service d'administration. Toutes les données proviennent exclusivement de la base de données.
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly AfroEventDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminService(AfroEventDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public AdminDashboardViewModel GetDashboardData()
        {
            // --- Statistiques réelles depuis la BDD ---
            var totalEvents    = _context.Events.Count();
            var totalTickets   = _context.Tickets.Count();
            var paidTickets    = _context.Tickets.Where(t => t.IsPaid).Include(t => t.Event).ToList();
            var totalRevenue   = paidTickets.Sum(t => t.Event?.Price ?? 0);
            var totalUsers     = _context.Users.Count();

            // --- Organisateurs en attente (persistés en BDD) ---
            var usersEnAttente = _context.Users
                .Where(u => u.OrganizerStatus == OrganizerStatus.EnAttente)
                .OrderBy(u => u.RegistrationDate)
                .ToList();

            var organisateursEnAttente = usersEnAttente.Select(u => new OrganisateurItemViewModel
            {
                Id           = u.Id,
                NomComplet   = u.FullName,
                NomOrganisation = u.OrganizationName,
                Email        = u.Email ?? string.Empty,
                Telephone    = u.PhoneNumber ?? string.Empty,
                DateDemande  = u.RegistrationDate,
                Statut       = "En attente"
            }).ToList();

            // --- Activités récentes (tickets récents + approbations) ---
            var recentTickets = _context.Tickets
                .Include(t => t.Event)
                .OrderByDescending(t => t.PurchaseDate)
                .Take(5)
                .ToList();

            var activites = recentTickets.Select(t => new ActiviteRecenteViewModel
            {
                Description  = $"Billet émis pour \"{t.Event?.Title ?? "Événement"}\"",
                DateHeure    = t.PurchaseDate,
                Type         = "Billet",
                TypeBadgeClass = "bg-info text-white"
            }).ToList();

            // Ajouter les organisateurs récemment approuvés
            var recentApprouves = _context.Users
                .Where(u => u.OrganizerStatus == OrganizerStatus.Approuve)
                .OrderByDescending(u => u.RegistrationDate)
                .Take(3)
                .ToList();

            foreach (var org in recentApprouves)
            {
                activites.Add(new ActiviteRecenteViewModel
                {
                    Description  = $"Organisateur approuvé : {org.OrganizationName} ({org.FullName})",
                    DateHeure    = org.RegistrationDate,
                    Type         = "Organisateur",
                    TypeBadgeClass = "bg-success text-white"
                });
            }

            activites = activites.OrderByDescending(a => a.DateHeure).Take(8).ToList();

            // --- Revenus par catégorie ---
            var revenusParCategorie = _context.Tickets
                .Where(t => t.IsPaid)
                .Include(t => t.Event)
                    .ThenInclude(e => e!.Category)
                .AsEnumerable()
                .GroupBy(t => t.Event?.Category?.Name ?? "Autre")
                .Select(g =>
                {
                    var montant = g.Sum(t => t.Event?.Price ?? 0);
                    return new RevenuParCategorieViewModel
                    {
                        Categorie     = g.Key,
                        Montant       = montant,
                        NombreBillets = g.Count()
                    };
                })
                .OrderByDescending(r => r.Montant)
                .ToList();

            // Calculer les pourcentages
            var totalMontant = revenusParCategorie.Sum(r => r.Montant);
            foreach (var r in revenusParCategorie)
            {
                r.Pourcentage = totalMontant > 0
                    ? Math.Round((double)(r.Montant / totalMontant) * 100, 1)
                    : 0;
            }

            return new AdminDashboardViewModel
            {
                TotalOrganisateurs         = totalUsers,
                TotalEvenements            = totalEvents,
                TotalBilletsVendus         = totalTickets,
                TotalRevenusSimules        = totalRevenue,
                OrganisateursEnAttenteCount = organisateursEnAttente.Count,
                OrganisateursEnAttente     = organisateursEnAttente,
                ActivitesRecentes          = activites,
                RevenusParCategorie        = revenusParCategorie
            };
        }

        public bool ApproveOrganizer(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.OrganizerStatus = OrganizerStatus.Approuve;
            _context.SaveChanges();

            // Ajouter au rôle Organisateur via UserManager (synchrone)
            // Note : la mise à jour du rôle est idéalement faite en async dans le contrôleur
            return true;
        }

        public bool RejectOrganizer(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}
