using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour la gestion du catalogue et des événements.
    /// </summary>
    public interface IEventService
    {
        List<EventFormViewModel> GetAllEvents();
        EventFormViewModel GetEventById(Guid id);
        void CreateEvent(EventFormViewModel model);
        void UpdateEvent(Guid id, EventFormViewModel model);
        Task ReservePlaceAsync(Guid id);
    }
}
