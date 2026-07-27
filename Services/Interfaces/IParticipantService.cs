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

        /// <summary>
        /// Traite le paiement simulé et génère un billet persisté en base.
        /// </summary>
        /// <param name="eventName">Nom de l'événement.</param>
        /// <param name="participantName">Nom affiché du participant.</param>
        /// <param name="participantEmail">Email du participant.</param>
        /// <param name="participantUserId">ID Identity du participant connecté (pas l'email).</param>
        /// <param name="passType">Type de pass sélectionné.</param>
        Task<TicketResultViewModel> ProcessPaymentAsync(
            Guid eventId,
            string participantName,
            string participantEmail,
            string participantUserId,
            string passType);

        byte[] GenerateTicketDownloadContent(string eventName, string participantName, string ticketId);
        string GenerateQrSvg(string value);
        System.Collections.Generic.List<TicketResultViewModel> GetTicketsForParticipant(string participantUserId);
        TicketResultViewModel GetTicketDetails(Guid ticketId);
    }

    public class TicketResultViewModel
    {
        public string TicketId { get; set; } = string.Empty;
        // Données de l'événement réel
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        public decimal EventPrice { get; set; }
        // Données du participant
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public string ParticipantPass { get; set; } = string.Empty;
        public string QrSvg { get; set; } = string.Empty;
        public string NotificationMessage { get; set; } = string.Empty;
    }
}
