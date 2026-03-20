using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class HomeController : Controller
{
    private readonly ISavingAccountService _savingAccountService;
    private readonly ITransactionService _transactionService;

    public HomeController(ISavingAccountService savingAccountService,
                          ITransactionService transactionService)
    {
        _savingAccountService = savingAccountService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Cuenta principal
        var mainAccountResult = await _savingAccountService.GetMainAccountByUserIdAsync(userId);

        if (mainAccountResult.IsFailure)
        {
            ViewBag.Error = mainAccountResult.GeneralError;
            return View();
        }

        var mainAccount = mainAccountResult.Value;

        // Últimas transacciones
        var transactionsResult = await _transactionService
            .GetTransactionsByAccountAsync(mainAccount!.Id, 1, 5);

        ViewBag.Account = mainAccount;
        ViewBag.Transactions = transactionsResult.IsSuccess
            ? transactionsResult.Value!.Items
            : new List<TransactionDto>();


        return View();
    }
}
