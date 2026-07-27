using System;
using System.Collections.Generic;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour l'espace organisateur.
    /// Toutes les méthodes filtrent par organizerId pour l'isolation des données.
    /// </summary>
    public interface IOrganizerService
    {
        OrganizerDashboardViewModel GetDashboardData(string organizerId, string organizerName);
        List<OrganizerEventSummaryViewModel> GetOrganizerEvents(string organizerId);
        List<AttendeeViewModel> GetAttendeesForEvent(Guid eventId);
        bool CheckInAttendee(Guid ticketId);
    }
}
