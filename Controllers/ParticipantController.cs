using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    [Authorize]
    public class ParticipantController : Controller
    {
        private readonly IParticipantService _participantService;
        private readonly UserManager<AppUser> _userManager;

        public ParticipantController(IParticipantService participantService, UserManager<AppUser> userManager)
        {
            _participantService = participantService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> SInscrire(string nom)
        {
            ViewBag.EventName = nom ?? "Événement";
            var model = new ParticipantInscriptionViewModel();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var fullName = $"{user.FirstName} {user.LastName}".Trim();
                model.NomComplet = !string.IsNullOrWhiteSpace(fullName) ? fullName : (user.UserName ?? string.Empty);
                model.Email = user.Email ?? string.Empty;
            }

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

            var eventName = _participantService.ProcessRegistration(nom ?? "Événement", model);

            TempData["ParticipantName"] = model.NomComplet;
            TempData["ParticipantEmail"] = model.Email;
            TempData["ParticipantPass"] = model.TypeP.ToString();
            TempData["EventName"] = eventName;
            TempData["PaymentStatus"] = "En attente";
            TempData["SuccessMessage"] = "Votre inscription a bien été enregistrée. Vous pouvez maintenant finaliser votre paiement.";

            return RedirectToAction(nameof(Confirmation), new { nom = eventName });
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
            var eventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            var participantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            var participantPass = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";

            var result = await _participantService.ProcessPaymentAsync(eventName, participantName, participantEmail, participantPass);

            TempData["EventName"] = result.EventName;
            TempData["ParticipantName"] = result.ParticipantName;
            TempData["ParticipantEmail"] = result.ParticipantEmail;
            TempData["ParticipantPass"] = result.ParticipantPass;
            TempData["PaymentStatus"] = "Payé";
            TempData["TicketId"] = result.TicketId;
            TempData["QrSvg"] = result.QrSvg;
            TempData["SuccessMessage"] = "Paiement simulé réussi. Votre e-billet est prêt.";

            HttpContext.Session.SetString("PaymentNotification", result.NotificationMessage);

            return RedirectToAction(nameof(Ticket), new { nom = eventName });
        }

        [HttpGet]
        public async Task<IActionResult> PayGet(string nom)
        {
            return await Pay(nom);
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
            
            var ticketId = ViewBag.TicketId.ToString();
            ViewBag.QrSvg = TempData.Peek("QrSvg")?.ToString() ?? _participantService.GenerateQrSvg(ticketId);
            ViewBag.SuccessMessage = TempData.Peek("SuccessMessage")?.ToString() ?? "Votre e-billet est prêt.";
            return View();
        }

        [HttpGet]
        public IActionResult DownloadTicket(string nom)
        {
            var eventName = nom ?? TempData.Peek("EventName")?.ToString() ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            var ticketId = TempData.Peek("TicketId")?.ToString() ?? "TKT-000000";

            var fileBytes = _participantService.GenerateTicketDownloadContent(eventName, participantName, ticketId);
            return File(fileBytes, "text/plain", $"billet-{ticketId}.txt");
        }
    }
}
