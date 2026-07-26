using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;
using Microsoft.AspNetCore.Http;

namespace AfroEvent.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<AfroEvent.Hubs.EventHub> _hubContext;

        public ParticipantController(Microsoft.AspNetCore.SignalR.IHubContext<AfroEvent.Hubs.EventHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpGet]
        public IActionResult SInscrire(string nom)
        {
            ViewBag.EventName = nom ?? "Événement";
            var model = new ParticipantInscriptionViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SInscrire(string nom, ParticipantInscriptionViewModel model)
        {
            ViewBag.EventName = nom ?? "Événement";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["ParticipantName"] = model.NomComplet;
            TempData["ParticipantEmail"] = model.Email;
            TempData["ParticipantPass"] = model.TypeP.ToString();
            TempData["EventName"] = nom ?? "Événement";
            TempData["PaymentStatus"] = "En attente";
            TempData["SuccessMessage"] = "Votre inscription a bien été enregistrée. Vous pouvez maintenant finaliser votre paiement.";

            return RedirectToAction(nameof(Confirmation), new { nom });
        }

        [HttpGet]
        public IActionResult Confirmation(string nom)
        {
            ViewBag.EventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            ViewBag.ParticipantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            ViewBag.ParticipantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            ViewBag.ParticipantPass = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";
            ViewBag.PaymentStatus = TempData.Peek("PaymentStatus")?.ToString() ?? "En attente";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(string nom)
        {
            var ticketId = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var eventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            var participantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            var participantPass = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";

            TempData["EventName"] = eventName;
            TempData["ParticipantName"] = participantName;
            TempData["ParticipantEmail"] = participantEmail;
            TempData["ParticipantPass"] = participantPass;
            TempData["PaymentStatus"] = "Payé";
            TempData["TicketId"] = ticketId;
            // Do not store large SVG in TempData (avoids oversized cookies / HTTP 431).
            TempData["SuccessMessage"] = "Paiement simulé réussi. Votre e-billet est prêt.";
            // Save a homepage notification in session so the user sees it on the Index page
            var note = $"Paiement reçu pour {eventName} — Billet {ticketId}.";
            HttpContext.Session.SetString("PaymentNotification", note);
            // send real-time notification via SignalR
            await _hubContext.Clients.All.SendCoreAsync("ReceiveNotification", new object[] { note });

            return RedirectToAction(nameof(Ticket), new { nom = eventName });
        }

        [HttpGet]
        public async Task<IActionResult> PayGet(string nom)
        {
            var ticketId = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var eventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            var participantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            var participantPass = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";

            TempData["EventName"] = eventName;
            TempData["ParticipantName"] = participantName;
            TempData["ParticipantEmail"] = participantEmail;
            TempData["ParticipantPass"] = participantPass;
            TempData["PaymentStatus"] = "Payé";
            TempData["TicketId"] = ticketId;
            // Generate QR on the Ticket view instead of storing SVG in TempData (prevents large cookie headers)
            TempData["SuccessMessage"] = "Paiement simulé réussi. Votre e-billet est prêt.";
            // Save a homepage notification in session so the user sees it on the Index page
            var note = $"Paiement reçu pour {eventName} — Billet {ticketId}.";
            HttpContext.Session.SetString("PaymentNotification", note);
            await _hubContext.Clients.All.SendCoreAsync("ReceiveNotification", new object[] { note });

            return RedirectToAction(nameof(Ticket), new { nom = eventName });
        }

        [HttpGet]
        public IActionResult Ticket(string nom)
        {
            ViewBag.EventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            ViewBag.ParticipantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            ViewBag.ParticipantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            ViewBag.ParticipantPass = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";
            ViewBag.PaymentStatus = TempData.Peek("PaymentStatus")?.ToString() ?? "Payé";
            ViewBag.TicketId = TempData.Peek("TicketId")?.ToString() ?? "TKT-000000";
            ViewBag.QrSvg = TempData.Peek("QrSvg")?.ToString() ?? GenerateQrSvg(ViewBag.TicketId.ToString());
            ViewBag.SuccessMessage = TempData.Peek("SuccessMessage")?.ToString() ?? "Votre e-billet est prêt.";
            return View();
        }

        [HttpGet]
        public IActionResult DownloadTicket(string nom)
        {
            var eventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            var ticketId = TempData.Peek("TicketId")?.ToString() ?? "TKT-000000";
            var content = $"Billet AfroEvent\nÉvénement : {eventName}\nParticipant : {participantName}\nTicket ID : {ticketId}\nStatut : Payé";
            var bytes = Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", $"billet-{ticketId}.txt");
        }

        private static string GenerateQrSvg(string value)
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
            sb.Append("<svg xmlns='http://www.w3.org/2000/svg' width='220' height='220' viewBox='0 0 21 21'>" );
            sb.Append("<rect width='21' height='21' fill='white' />" );
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
