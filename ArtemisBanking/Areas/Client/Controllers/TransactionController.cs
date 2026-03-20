using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;

    public TransactionController(ITransactionService transactionService,
                                 ISavingAccountService savingAccountService)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
    }

    public async Task<IActionResult> Index(string? accountNumber, int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Obtener cuenta principal si no se selecciona ninguna
        if (string.IsNullOrEmpty(accountNumber))
        {
            var mainAccount = await _savingAccountService.GetMainAccountByUserIdAsync(userId!);

            if (mainAccount.IsSuccess)
                accountNumber = mainAccount.Value!.Id;
        }

        var result = await _transactionService.GetTransactionsByAccountAsync(accountNumber!, page, 10);

        if (result.IsFailure)
        {
            ViewBag.Error = result.GeneralError;
            return View();
        }

        ViewBag.AccountNumber = accountNumber;
        return View(result.Value);
    }
}
