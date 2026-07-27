using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour la couche événements (CRUD, réservation, temps réel).
    /// </summary>
    public interface IEventService
    {
        List<EventFormViewModel> GetAllEvents();
        List<EventFormViewModel> GetUpcomingEvents(int count = 6);
        EventFormViewModel? GetEventById(Guid id);
        void CreateEvent(EventFormViewModel model, string organizerId);
        void UpdateEvent(Guid id, EventFormViewModel model);
        void DeleteEvent(Guid id);
        Task ReservePlaceAsync(Guid id);
        int GetAvailablePlaces(Guid eventId);
    }
}
