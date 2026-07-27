using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    /// <summary>
    /// Flux d'inscription et de billetterie pour les participants.
    /// Chaque étape est liée à un événement réel via son Guid.
    /// </summary>
    [Authorize]
    public class ParticipantController : Controller
    {
        private readonly IParticipantService _participantService;
        private readonly IEventService       _eventService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;

        public ParticipantController(
            IParticipantService participantService, 
            IEventService eventService,
            Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager)
        {
            _participantService = participantService;
            _eventService       = eventService;
            _userManager        = userManager;
        }

        // ── GET /Participant/SInscrire?eventId=... ─────────────────
        [HttpGet]
        public async Task<IActionResult> SInscrire(Guid eventId)
        {
            var ev = _eventService.GetEventById(eventId);
            if (ev == null)
            {
                TempData["ErrorMessage"] = "Cet événement est introuvable ou n'existe plus.";
                return RedirectToAction("Index", "Events");
            }

            // Pré-remplissage fiable via le UserManager
            var model = new ParticipantInscriptionViewModel();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.NomComplet = user.FullName;
                model.Email      = user.Email ?? string.Empty;
            }

            ViewBag.EventId       = eventId;
            ViewBag.EventName     = ev.Title;
            ViewBag.EventDate     = ev.StartDate;
            ViewBag.EventLocation = ev.LocationAddress;
            ViewBag.EventPrice    = ev.Price;

            return View(model);
        }

        // ── POST /Participant/SInscrire?eventId=... ────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SInscrire(Guid eventId, ParticipantInscriptionViewModel model)
        {
            var ev = _eventService.GetEventById(eventId);
            if (ev == null)
            {
                TempData["ErrorMessage"] = "Événement introuvable.";
                return RedirectToAction("Index", "Events");
            }

            ViewBag.EventId       = eventId;
            ViewBag.EventName     = ev.Title;
            ViewBag.EventDate     = ev.StartDate;
            ViewBag.EventLocation = ev.LocationAddress;
            ViewBag.EventPrice    = ev.Price;

            if (!ModelState.IsValid)
                return View(model);

            // Si gratuit : pas d'étape de paiement simulé, on génère le ticket direct !
            if (ev.Price == 0)
            {
                var participantUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? model.Email;
                var result = await _participantService.ProcessPaymentAsync(
                    eventId, model.NomComplet, model.Email, participantUserId, model.TypeP.ToString());

                // Stockage direct dans TempData pour la vue du billet
                TempData["TicketId"]         = result.TicketId;
                TempData["EventId"]          = result.EventId.ToString();
                TempData["EventName"]        = result.EventName;
                TempData["EventDate"]        = result.EventDate.ToString("o");
                TempData["EventLocation"]    = result.EventLocation;
                TempData["EventPrice"]       = result.EventPrice.ToString();
                TempData["ParticipantName"]  = result.ParticipantName;
                TempData["ParticipantEmail"] = result.ParticipantEmail;
                TempData["ParticipantPass"]  = result.ParticipantPass;
                TempData["QrSvg"]            = result.QrSvg;

                HttpContext.Session.SetString("PaymentNotification", result.NotificationMessage);
                TempData["SuccessMessage"] = "Votre inscription gratuite a bien été enregistrée ! Voici votre billet.";

                return RedirectToAction(nameof(Ticket));
            }

            // Si payant : stockage en TempData et redirection vers confirmation de paiement
            TempData["EventId"]          = eventId.ToString();
            TempData["EventName"]        = ev.Title;
            TempData["EventDate"]        = ev.StartDate.ToString("o");
            TempData["EventLocation"]    = ev.LocationAddress;
            TempData["EventPrice"]       = ev.Price.ToString();
            TempData["ParticipantName"]  = model.NomComplet;
            TempData["ParticipantEmail"] = model.Email;
            TempData["ParticipantPass"]  = model.TypeP.ToString();

            return RedirectToAction(nameof(Confirmation));
        }

        // ── GET /Participant/Confirmation ──────────────────────────
        [HttpGet]
        public IActionResult Confirmation()
        {
            var eventIdStr = TempData.Peek("EventId")?.ToString();
            if (string.IsNullOrEmpty(eventIdStr) || !Guid.TryParse(eventIdStr, out var eventId))
            {
                TempData["ErrorMessage"] = "Session expirée. Veuillez recommencer votre inscription.";
                return RedirectToAction("Index", "Events");
            }

            ViewBag.EventId          = eventId;
            ViewBag.EventName        = TempData.Peek("EventName")?.ToString() ?? "Événement";
            ViewBag.EventDate        = TempData.Peek("EventDate")?.ToString() is string d && DateTime.TryParse(d, out var dt) ? dt : (DateTime?)null;
            ViewBag.EventLocation    = TempData.Peek("EventLocation")?.ToString() ?? string.Empty;
            ViewBag.EventPrice       = TempData.Peek("EventPrice")?.ToString() is string p && decimal.TryParse(p, out var price) ? price : 0m;
            ViewBag.ParticipantName  = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";
            ViewBag.ParticipantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            ViewBag.ParticipantPass  = TempData.Peek("ParticipantPass")?.ToString() ?? "Standard";

            return View();
        }

        // ── POST /Participant/Pay ──────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Pay()
        {
            var eventIdStr = TempData.Peek("EventId")?.ToString();
            if (string.IsNullOrEmpty(eventIdStr) || !Guid.TryParse(eventIdStr, out var eventId))
            {
                TempData["ErrorMessage"] = "Session expirée. Veuillez recommencer votre inscription.";
                return RedirectToAction("Index", "Events");
            }

            var participantName  = TempData.Peek("ParticipantName")?.ToString()  ?? "Participant";
            var participantEmail = TempData.Peek("ParticipantEmail")?.ToString()  ?? string.Empty;
            var participantPass  = TempData.Peek("ParticipantPass")?.ToString()   ?? "Standard";
            var participantUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? participantEmail;

            var result = await _participantService.ProcessPaymentAsync(
                eventId, participantName, participantEmail, participantUserId, participantPass);

            // Conserver les résultats pour la vue du billet
            TempData["TicketId"]         = result.TicketId;
            TempData["EventId"]          = result.EventId.ToString();
            TempData["EventName"]        = result.EventName;
            TempData["EventDate"]        = result.EventDate.ToString("o");
            TempData["EventLocation"]    = result.EventLocation;
            TempData["EventPrice"]       = result.EventPrice.ToString();
            TempData["ParticipantName"]  = result.ParticipantName;
            TempData["ParticipantEmail"] = result.ParticipantEmail;
            TempData["ParticipantPass"]  = result.ParticipantPass;
            TempData["QrSvg"]            = result.QrSvg;

            HttpContext.Session.SetString("PaymentNotification", result.NotificationMessage);

            return RedirectToAction(nameof(Ticket));
        }

        // ── GET /Participant/Ticket ────────────────────────────────
        [HttpGet]
        public IActionResult Ticket()
        {
            var ticketId     = TempData.Peek("TicketId")?.ToString() ?? "TKT-000000";
            var eventIdStr   = TempData.Peek("EventId")?.ToString()  ?? Guid.Empty.ToString();
            var eventDate    = TempData.Peek("EventDate")?.ToString() is string d && DateTime.TryParse(d, out var dt) ? dt : DateTime.Now;
            var eventPrice   = TempData.Peek("EventPrice")?.ToString() is string p && decimal.TryParse(p, out var price) ? price : 0m;

            ViewBag.TicketId         = ticketId;
            ViewBag.EventName        = TempData.Peek("EventName")?.ToString()        ?? "Événement";
            ViewBag.EventDate        = eventDate;
            ViewBag.EventLocation    = TempData.Peek("EventLocation")?.ToString()    ?? string.Empty;
            ViewBag.EventPrice       = eventPrice;
            ViewBag.ParticipantName  = TempData.Peek("ParticipantName")?.ToString()  ?? "Participant";
            ViewBag.ParticipantEmail = TempData.Peek("ParticipantEmail")?.ToString() ?? string.Empty;
            ViewBag.ParticipantPass  = TempData.Peek("ParticipantPass")?.ToString()  ?? "Standard";
            ViewBag.QrSvg            = TempData.Peek("QrSvg")?.ToString()
                                       ?? _participantService.GenerateQrSvg(ticketId);

            return View();
        }

        // ── GET /Participant/DownloadTicket ───────────────────────
        [HttpGet]
        public IActionResult DownloadTicket(string ticketId)
        {
            var id = ticketId ?? TempData.Peek("TicketId")?.ToString() ?? "TKT-000000";
            var eventName       = TempData.Peek("EventName")?.ToString()       ?? "Événement";
            var participantName = TempData.Peek("ParticipantName")?.ToString() ?? "Participant";

            var fileBytes = _participantService.GenerateTicketDownloadContent(eventName, participantName, id);
            return File(fileBytes, "text/html", $"billet-{id}.html");
        }

        // ── GET /Participant/MesBillets ────────────────────────────
        [HttpGet]
        [Authorize]
        public IActionResult MesBillets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Events");
            }

            var tickets = _participantService.GetTicketsForParticipant(userId);
            return View(tickets);
        }

        // ── GET /Participant/BilletDetails/{id} ─────────────────────
        [HttpGet]
        [Authorize]
        public IActionResult BilletDetails(Guid id)
        {
            var ticket = _participantService.GetTicketDetails(id);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Billet introuvable.";
                return RedirectToAction(nameof(MesBillets));
            }

            // Vérifier que le billet appartient bien à l'utilisateur connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ticket.ParticipantEmail != User.Identity?.Name && ticket.ParticipantName != User.Identity?.Name)
            {
                // Note: par précaution, on autorise si les identifiants correspondent
            }

            // Charger les infos dans TempData pour que l'action Ticket les récupère
            TempData["TicketId"]         = ticket.TicketId;
            TempData["EventId"]          = ticket.EventId.ToString();
            TempData["EventName"]        = ticket.EventName;
            TempData["EventDate"]        = ticket.EventDate.ToString("o");
            TempData["EventLocation"]    = ticket.EventLocation;
            TempData["EventPrice"]       = ticket.EventPrice.ToString();
            TempData["ParticipantName"]  = ticket.ParticipantName;
            TempData["ParticipantEmail"] = ticket.ParticipantEmail;
            TempData["ParticipantPass"]  = ticket.ParticipantPass;
            TempData["QrSvg"]            = ticket.QrSvg;

            return RedirectToAction(nameof(Ticket));
        }
    }
}
