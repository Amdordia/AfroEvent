using System;
using System.Collections.Generic;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour l'espace organisateur et la logistique.
    /// </summary>
    public interface IOrganizerService
    {
        OrganizerDashboardViewModel GetDashboardData();
        List<OrganizerEventSummaryViewModel> GetOrganizerEvents();
        List<AttendeeViewModel> GetAttendeesForEvent(Guid eventId);
        bool CheckInAttendee(Guid ticketId);
    }
}
