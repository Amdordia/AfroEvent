using System;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Services.Interfaces;

namespace AfroEvent.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public IActionResult Dashboard()
    {
        var model = _adminService.GetDashboardData();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApprouverOrganisateur(Guid id)
    {
        var success = _adminService.ApproveOrganizer(id);
        if (success)
        {
            TempData["SuccesMessage"] = "L'organisateur a été approuvé avec succès.";
        }
        else
        {
            TempData["WarningMessage"] = "Organisateur non trouvé ou déjà traité.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RejeterOrganisateur(Guid id)
    {
        var success = _adminService.RejectOrganizer(id);
        if (success)
        {
            TempData["WarningMessage"] = "La demande d'organisateur a été rejetée.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}
