using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AfroEvent.Hubs;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    public class ParticipantService : IParticipantService
    {
        private readonly IHubContext<EventHub> _hubContext;

        public ParticipantService(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public string ProcessRegistration(string eventName, ParticipantInscriptionViewModel model)
        {
            // Business validation or registration storage logic
            return eventName ?? "Événement";
        }

        public async Task<TicketResultViewModel> ProcessPaymentAsync(string eventName, string participantName, string participantEmail, string passType)
        {
            var ticketId = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var note = $"Paiement reçu pour {eventName} — Billet {ticketId}.";

            // Emit SignalR Realtime notification
            await _hubContext.Clients.All.SendCoreAsync("ReceiveNotification", new object[] { note });

            var qrSvg = GenerateQrSvg(ticketId);

            return new TicketResultViewModel
            {
                TicketId = ticketId,
                EventName = eventName,
                ParticipantName = participantName,
                ParticipantEmail = participantEmail,
                ParticipantPass = passType,
                QrSvg = qrSvg,
                NotificationMessage = note
            };
        }

        public byte[] GenerateTicketDownloadContent(string eventName, string participantName, string ticketId)
        {
            var content = $"=== BILLET ÉLECTRONIQUE AFROEVENT ===\n" +
                          $"Événement      : {eventName}\n" +
                          $"Participant    : {participantName}\n" +
                          $"N° de Billet   : {ticketId}\n" +
                          $"Date d'émission: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                          $"Statut         : Payé / Valide\n" +
                          $"=====================================\n" +
                          $"Présentez ce billet (ou son QR Code) à l'entrée de l'événement.";

            return Encoding.UTF8.GetBytes(content);
        }

        public string GenerateQrSvg(string value)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            var cells = new bool[21, 21];
            var seed = BitConverter.ToUInt32(hash, 0);
            var random = new Random((int)seed);

            for (int y = 0; y < 21; y++)
            {
                for (int x = 0; x < 21; x++)
                {
                    if (x < 7 && y < 7 || x > 13 && y < 7 || x < 7 && y > 13)
                    {
                        cells[y, x] = true;
                        continue;
                    }

                    cells[y, x] = random.Next(0, 100) < 55;
                }
            }

            var sb = new StringBuilder();
            sb.Append("<svg xmlns='http://www.w3.org/2000/svg' width='220' height='220' viewBox='0 0 21 21'>");
            sb.Append("<rect width='21' height='21' fill='white' />");
            for (var y = 0; y < 21; y++)
            {
                for (var x = 0; x < 21; x++)
                {
                    if (!cells[y, x]) continue;
                    sb.Append($"<rect x='{x}' y='{y}' width='1' height='1' fill='black' />");
                }
            }
            sb.Append("</svg>");
            return sb.ToString();
        }
    }
}
