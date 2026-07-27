using System;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour la supervision de la plateforme et la gestion des organisateurs.
    /// Toutes les données proviennent de la base de données (aucune donnée statique).
    /// </summary>
    public interface IAdminService
    {
        AdminDashboardViewModel GetDashboardData();
        bool ApproveOrganizer(string userId);
        bool RejectOrganizer(string userId);
    }
}
