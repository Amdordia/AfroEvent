using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AfroEvent.Models;
using AfroEvent.Services.Interfaces;

namespace AfroEvent.Controllers;

/// <summary>
/// Tableau de bord administrateur. Accès strictement réservé au rôle Admin.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly UserManager<AppUser> _userManager;

    public AdminController(IAdminService adminService, UserManager<AppUser> userManager)
    {
        _adminService = adminService;
        _userManager  = userManager;
    }

    public IActionResult Dashboard()
    {
        var model = _adminService.GetDashboardData();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApprouverOrganisateur(string id)
    {
        var success = _adminService.ApproveOrganizer(id);
        if (success)
        {
            // Affecter le rôle Organisateur via Identity
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Organisateur"))
            {
                await _userManager.AddToRoleAsync(user, "Organisateur");
            }
            TempData["SuccessMessage"] = "L'organisateur a été approuvé et son accès activé.";
        }
        else
        {
            TempData["WarningMessage"] = "Utilisateur non trouvé ou déjà traité.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejeterOrganisateur(string id)
    {
        var success = _adminService.RejectOrganizer(id);
        if (success)
        {
            // Retirer le rôle Organisateur s'il l'avait déjà
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && await _userManager.IsInRoleAsync(user, "Organisateur"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Organisateur");
            }
            TempData["WarningMessage"] = "La demande d'organisateur a été rejetée.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}
