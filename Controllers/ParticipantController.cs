using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    public class ParticipantController : Controller
    {
        [HttpGet]
        public IActionResult SInscrire()
        {
            var model = new ParticipantInscriptionViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SInscrire(ParticipantInscriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["SuccessMessage"] = "Votre inscription a bien été enregistrée.";
            return RedirectToAction("SInscrire");
        }
    }
}
