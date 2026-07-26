using System;
using System.Threading.Tasks;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Interfaces
{
    /// <summary>
    /// Contrat de service pour l'inscription, le paiement simulé et la billetterie QR Code des participants.
    /// </summary>
    public interface IParticipantService
    {
        string ProcessRegistration(string eventName, ParticipantInscriptionViewModel model);
        Task<TicketResultViewModel> ProcessPaymentAsync(string eventName, string participantName, string participantEmail, string passType);
        byte[] GenerateTicketDownloadContent(string eventName, string participantName, string ticketId);
        string GenerateQrSvg(string value);
    }

    public class TicketResultViewModel
    {
        public string TicketId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public string ParticipantPass { get; set; } = string.Empty;
        public string QrSvg { get; set; } = string.Empty;
        public string NotificationMessage { get; set; } = string.Empty;
    }
}
