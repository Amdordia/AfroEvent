using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    /// <summary>
    /// ConferenceController — Maintenu pour compatibilité avec les anciens liens.
    /// Le flux d'inscription complet (Inscription → Paiement → Billet QR) 
    /// est géré par ParticipantController.
    /// </summary>
    [Authorize]
    public class ConferenceController : Controller
    {
        /// <summary>
        /// Redirige vers le catalogue d'événements principal.
        /// Les anciennes URL /Conference/SInscrire sont redirigées vers le flux Participant.
        /// </summary>
        [HttpGet]
        public IActionResult SInscrire()
        {
            // Redirection vers le catalogue — l'utilisateur choisira l'événement
            // puis accèdera au flux unifié Participant/SInscrire?nom={eventName}
            TempData["InfoMessage"] = "Sélectionnez un événement ci-dessous pour vous inscrire.";
            return RedirectToAction("Index", "Events");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SInscrire(ConferenceInscriptionViewModel model)
        {
            // Redirection vers le flux Participant unifié
            return RedirectToAction("Index", "Events");
        }
    }
}
