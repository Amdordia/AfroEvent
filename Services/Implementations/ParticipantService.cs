using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Hubs;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Services.Implementations
{
    /// <summary>
    /// Gestion des inscriptions et de la billetterie participante.
    /// Chaque billet est lié à un événement réel via son Guid.
    /// </summary>
    public class ParticipantService : IParticipantService
    {
        private readonly AfroEventDbContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        public ParticipantService(AfroEventDbContext context, IHubContext<EventHub> hubContext)
        {
            _context    = context;
            _hubContext = hubContext;
        }

        public string ProcessRegistration(string eventName, ParticipantInscriptionViewModel model)
        {
            return eventName ?? "Événement";
        }

        /// <summary>
        /// Génère et persiste un billet électronique après paiement simulé.
        /// L'événement est résolu via son ID — aucune recherche par titre fragile.
        /// </summary>
        public async Task<TicketResultViewModel> ProcessPaymentAsync(
            Guid eventId,
            string participantName,
            string participantEmail,
            string participantUserId,
            string passType)
        {
            // Charger l'événement réel depuis la BDD
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null)
            {
                throw new InvalidOperationException($"Événement introuvable : {eventId}");
            }

            var ticketId = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            var note     = $"Nouveau billet émis pour \"{ev.Title}\" — {ticketId}";

            // Hash QR Code sécurisé
            var qrHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ticketId)));

            // Persistance du billet en BDD
            var ticket = new TicketEntity
            {
                Id            = Guid.NewGuid(),
                QrCodeHash    = qrHash,
                IsPaid        = true,
                IsPresent     = false,
                PurchaseDate  = DateTime.UtcNow,
                EventId       = eventId,
                ParticipantId = participantUserId
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            // Notification temps réel
            await _hubContext.Clients.All.SendCoreAsync("ReceiveNotification", new object[] { note });

            return new TicketResultViewModel
            {
                TicketId            = ticketId,
                EventId             = eventId,
                EventName           = ev.Title,
                EventDate           = ev.StartDate,
                EventLocation       = ev.LocationAddress,
                EventPrice          = ev.Price,
                ParticipantName     = participantName,
                ParticipantEmail    = participantEmail,
                ParticipantPass     = passType,
                QrSvg               = GenerateQrSvg(ticketId),
                NotificationMessage = note
            };
        }

        public byte[] GenerateTicketDownloadContent(string eventName, string participantName, string ticketId)
        {
            var qrCodeSvg = GenerateQrSvg(ticketId);
            var now = DateTime.Now;

            var html = $@"<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Billet — {ticketId}</title>
    <link href='https://fonts.googleapis.com/css2?family=Outfit:wght@400;600;700;800&display=swap' rel='stylesheet' />
    <style>
        body {{
            font-family: 'Outfit', sans-serif;
            background-color: #F8FAFC;
            color: #1E293B;
            margin: 0;
            padding: 40px 20px;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }}
        .ticket-container {{
            background: #ffffff;
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.06);
            width: 100%;
            max-width: 550px;
            overflow: hidden;
            border: 1px solid #E2E8F0;
        }}
        .ticket-header {{
            background: linear-gradient(135deg, #111622 0%, #1A2236 60%, #2C1508 100%);
            padding: 30px;
            color: #ffffff;
            position: relative;
        }}
        .ticket-header::after {{
            content: '';
            position: absolute;
            top: -50px;
            right: -50px;
            width: 150px;
            height: 150px;
            background: radial-gradient(circle, rgba(255,183,3,0.15) 0%, transparent 70%);
            pointer-events: none;
        }}
        .logo-text {{
            font-weight: 800;
            font-size: 1.2rem;
            letter-spacing: 0.05em;
            margin-bottom: 20px;
        }}
        .logo-text span {{
            color: #FFB703;
        }}
        .event-title {{
            font-size: 1.6rem;
            font-weight: 700;
            margin: 0 0 10px 0;
            line-height: 1.2;
        }}
        .pass-type {{
            display: inline-block;
            background: rgba(255,183,3,0.2);
            color: #FFB703;
            border: 1px solid rgba(255,183,3,0.3);
            padding: 4px 12px;
            border-radius: 99px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }}
        .ticket-divider {{
            height: 24px;
            position: relative;
            background: #ffffff;
            display: flex;
            align-items: center;
        }}
        .ticket-divider::before {{
            content: '';
            position: absolute;
            left: -12px;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            background-color: #F8FAFC;
            border: 1px solid #E2E8F0;
        }}
        .ticket-divider::after {{
            content: '';
            position: absolute;
            right: -12px;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            background-color: #F8FAFC;
            border: 1px solid #E2E8F0;
        }}
        .divider-line {{
            flex: 1;
            border-top: 2px dashed #CBD5E1;
            margin: 0 20px;
        }}
        .ticket-body {{
            padding: 30px;
            background: #ffffff;
        }}
        .info-grid {{
            display: grid;
            grid-template-columns: 1fr;
            gap: 20px;
        }}
        @media(min-width: 480px) {{
            .info-grid {{
                grid-template-columns: 1fr 1fr;
            }}
        }}
        .info-label {{
            font-size: 0.75rem;
            text-transform: uppercase;
            font-weight: 600;
            color: #64748B;
            letter-spacing: 0.05em;
            margin-bottom: 4px;
        }}
        .info-value {{
            font-size: 1rem;
            font-weight: 700;
            color: #0F172A;
        }}
        .ticket-number {{
            background: #F1F5F9;
            color: #F37021;
            padding: 4px 10px;
            border-radius: 6px;
            font-family: monospace;
            font-size: 0.9rem;
            font-weight: bold;
        }}
        .qr-section {{
            text-align: center;
            margin-top: 30px;
            padding-top: 30px;
            border-top: 1px solid #F1F5F9;
        }}
        .qr-box {{
            display: inline-block;
            background: #ffffff;
            padding: 12px;
            border-radius: 12px;
            border: 2px solid #F1F5F9;
            box-shadow: 0 4px 12px rgba(0,0,0,0.02);
            margin-bottom: 10px;
        }}
        .qr-box svg {{
            width: 160px;
            height: 160px;
        }}
        .qr-text {{
            font-size: 0.75rem;
            color: #64748B;
            margin: 0;
        }}
        .btn-print {{
            display: block;
            width: 100%;
            text-align: center;
            background: #FFB703;
            color: #111622;
            border: none;
            padding: 14px 20px;
            font-size: 0.95rem;
            font-weight: 700;
            border-radius: 12px;
            cursor: pointer;
            transition: all 0.2s ease;
            margin-top: 25px;
            box-shadow: 0 4px 12px rgba(255,183,3,0.2);
        }}
        .btn-print:hover {{
            background: #F37021;
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(243,112,33,0.3);
        }}
        @media print {{
            body {{
                background: #ffffff;
                padding: 0;
            }}
            .ticket-container {{
                box-shadow: none;
                max-width: 100%;
                border: none;
            }}
            .btn-print {{
                display: none;
            }}
        }}
    </style>
</head>
<body>
    <div class='ticket-container'>
        <div class='ticket-header'>
            <div class='logo-text'>Afro<span>Event</span></div>
            <h1 class='event-title'>{eventName}</h1>
            <span class='pass-type'>Accès Régulier</span>
        </div>
        
        <div class='ticket-divider'>
            <div class='divider-line'></div>
        </div>

        <div class='ticket-body'>
            <div class='info-grid'>
                <div>
                    <div class='info-label'>Participant</div>
                    <div class='info-value'>{participantName}</div>
                </div>
                <div>
                    <div class='info-label'>Numéro de billet</div>
                    <div><span class='ticket-number'>{ticketId}</span></div>
                </div>
                <div>
                    <div class='info-label'>Date de téléchargement</div>
                    <div class='info-value'>{now:dd/MM/yyyy HH:mm}</div>
                </div>
                <div>
                    <div class='info-label'>Statut du billet</div>
                    <div class='info-value' style='color:#10B981;'>✓ Confirmé / Valide</div>
                </div>
            </div>

            <div class='qr-section'>
                <div class='qr-box'>
                    {qrCodeSvg}
                </div>
                <p class='qr-text'>Présentez ce QR Code lors du contrôle d'accès</p>
            </div>

            <button class='btn-print' onclick='window.print()'>
                🖨️ Imprimer / Sauvegarder en PDF
            </button>
        </div>
    </div>
</body>
</html>";

            return Encoding.UTF8.GetBytes(html);
        }

        public string GenerateQrSvg(string value)
        {
            var hash   = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            var cells  = new bool[21, 21];
            var seed   = BitConverter.ToUInt32(hash, 0);
            var random = new Random((int)seed);

            for (int y = 0; y < 21; y++)
            {
                for (int x = 0; x < 21; x++)
                {
                    if ((x < 7 && y < 7) || (x > 13 && y < 7) || (x < 7 && y > 13))
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
        public System.Collections.Generic.List<TicketResultViewModel> GetTicketsForParticipant(string participantUserId)
        {
            var tickets = _context.Tickets
                .Include(t => t.Event)
                .Where(t => t.ParticipantId == participantUserId)
                .OrderByDescending(t => t.PurchaseDate)
                .ToList();

            var user = _context.Users.FirstOrDefault(u => u.Id == participantUserId);
            var participantName = user?.FullName ?? "Participant";
            var participantEmail = user?.Email ?? string.Empty;

            return tickets.Select(t => new TicketResultViewModel
            {
                TicketId = t.Id.ToString().ToUpper(),
                EventId = t.EventId,
                EventName = t.Event?.Title ?? "Événement",
                EventDate = t.Event?.StartDate ?? DateTime.Now,
                EventLocation = t.Event?.LocationAddress ?? string.Empty,
                EventPrice = t.Event?.Price ?? 0m,
                ParticipantName = participantName,
                ParticipantEmail = participantEmail,
                ParticipantPass = (t.Event?.Price > 15000) ? "VIP" : "Standard",
                QrSvg = GenerateQrSvg(t.Id.ToString()),
                NotificationMessage = string.Empty
            }).ToList();
        }

        public TicketResultViewModel GetTicketDetails(Guid ticketId)
        {
            var ticket = _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null) return null;

            var user = _context.Users.FirstOrDefault(u => u.Id == ticket.ParticipantId);
            var participantName = user?.FullName ?? "Participant";
            var participantEmail = user?.Email ?? string.Empty;

            return new TicketResultViewModel
            {
                TicketId = ticket.Id.ToString().ToUpper(),
                EventId = ticket.EventId,
                EventName = ticket.Event?.Title ?? "Événement",
                EventDate = ticket.Event?.StartDate ?? DateTime.Now,
                EventLocation = ticket.Event?.LocationAddress ?? string.Empty,
                EventPrice = ticket.Event?.Price ?? 0m,
                ParticipantName = participantName,
                ParticipantEmail = participantEmail,
                ParticipantPass = (ticket.Event?.Price > 15000) ? "VIP" : "Standard",
                QrSvg = GenerateQrSvg(ticket.Id.ToString()),
                NotificationMessage = string.Empty
            };
        }
    }
}
