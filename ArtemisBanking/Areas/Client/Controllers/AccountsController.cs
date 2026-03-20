using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class AccountsController : Controller
{
    private readonly ISavingAccountService _savingAccountService;

    public AccountsController(ISavingAccountService savingAccountService)
    {
        _savingAccountService = savingAccountService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Index", "Login", new { area = "" });

        var accounts = await _savingAccountService.GetByUserIdAsync(userId);

        return View(accounts);
    }
}
