using System;
using Microsoft.AspNetCore.Identity;

namespace AfroEvent.Models
{
    /// <summary>
    /// Statut de validation d'un compte organisateur.
    /// </summary>
    public enum OrganizerStatus
    {
        /// <summary>Compte standard (participant ou admin).</summary>
        NonApplicable = 0,
        /// <summary>Demande d'organisateur soumise, en attente de validation admin.</summary>
        EnAttente = 1,
        /// <summary>Compte organisateur approuvé par un administrateur.</summary>
        Approuve = 2,
        /// <summary>Demande d'organisateur rejetée.</summary>
        Rejete = 3
    }

    /// <summary>
    /// Entité utilisateur AfroEvent. Étend IdentityUser avec les champs métier spécifiques.
    /// </summary>
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        /// <summary>Nom de l'organisation (pour les comptes Organisateur).</summary>
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>Statut de validation du compte organisateur.</summary>
        public OrganizerStatus OrganizerStatus { get; set; } = OrganizerStatus.NonApplicable;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        /// <summary>Nom complet calculé pour l'affichage.</summary>
        public string FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? (UserName ?? Email ?? "Utilisateur")
            : $"{FirstName} {LastName}".Trim();
    }
}