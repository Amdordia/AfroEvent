using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    public class ConferenceController : Controller
    {
        [HttpGet]
        public IActionResult SInscrire()
        {
            var model = new ConferenceInscriptionViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SInscrire(ConferenceInscriptionViewModel model)
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
