using System;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour la supervision de la plateforme et la gestion des organisateurs.
    /// </summary>
    public interface IAdminService
    {
        AdminDashboardViewModel GetDashboardData();
        bool ApproveOrganizer(Guid id);
        bool RejectOrganizer(Guid id);
    }
}
