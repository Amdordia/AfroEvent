using Microsoft.AspNetCore.Mvc;
using AfroEvent.ViewModels;

namespace AfroEvent.Controllers
{
    public class ProjetController : Controller
    {
        // (GET)
        [HttpGet]
        public IActionResult Creer()
        {
            var model = new ProjetSoumissionViewModel();
            return View(model);
        }

        // (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creer(ProjetSoumissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["SuccesMessage"] = $"Le projet '{model.Titre}' a été soumis avec succès !";
            return RedirectToAction("Creer");
        }
    }
}