using ArtemisBanking.Core.Application.Dtos.Transfer;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class TransferController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly IBeneficiaryService _beneficiaryService;

    public TransferController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        IBeneficiaryService beneficiaryService)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _beneficiaryService = beneficiaryService;
    }

    // 🔹 GET
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var accounts = await _savingAccountService.GetByUserIdAsync(userId!);
        var beneficiaries = await _beneficiaryService.GetByUserIdAsync(userId!);

        ViewBag.Accounts = accounts;
        ViewBag.Beneficiaries = beneficiaries;

        return View();
    }

    // 🔹 POST
    [HttpPost]
    public async Task<IActionResult> Index(TransferDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _transactionService.TransferAsync(userId!, dto);

        // 🔁 IMPORTANTE: recargar datos SIEMPRE
        var accounts = await _savingAccountService.GetByUserIdAsync(userId!);
        var beneficiaries = await _beneficiaryService.GetByUserIdAsync(userId!);

        ViewBag.Accounts = accounts;
        ViewBag.Beneficiaries = beneficiaries;

        if (result.IsFailure)
        {
            ViewBag.Error = result.GeneralError;
            return View(dto);
        }

        ViewBag.Success = "Transferencia realizada correctamente ✅";
        return View();
    }
}
