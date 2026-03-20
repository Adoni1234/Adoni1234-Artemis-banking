using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class PaymentController : Controller
{
    private readonly ISavingAccountService _savingAccountService;
    private readonly ICreditCardService _creditCardService;
    private readonly ILoanService _loanService;
    private readonly ITransactionService _transactionService;

    public PaymentController(
        ISavingAccountService savingAccountService,
        ICreditCardService creditCardService,
        ILoanService loanService,
        ITransactionService transactionService)
    {
        _savingAccountService = savingAccountService;
        _creditCardService = creditCardService;
        _loanService = loanService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        ViewBag.Accounts = await _savingAccountService.GetByUserIdAsync(userId);
        ViewBag.Cards = await _creditCardService.GetByUserIdAsync(userId);
        ViewBag.Loans = await _loanService.GetByUserIdAsync(userId);

        return View();
    }

    // ================= TARJETA =================
    [HttpPost]
    public async Task<IActionResult> PayCreditCard(string accountNumber, string cardNumber, decimal amount)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var dto = new TellerCreditCardPaymentDto
        {
            SourceAccountNumber = accountNumber,
            CreditCardNumber = cardNumber,
            Amount = amount,
            TellerId = userId // cliente actúa como "teller"
        };

        var result = await _transactionService.ProcessTellerCreditCardPaymentAsync(dto);

        if (result.IsFailure)
        {
            ViewBag.Error = result.GeneralError;
            return await Index();
        }

        ViewBag.Success = "Pago de tarjeta realizado ✅";
        return await Index();
    }

    // ================= PRÉSTAMO =================
    [HttpPost]
    public async Task<IActionResult> PayLoan(string accountNumber, string loanNumber, decimal amount)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var dto = new TellerLoanPaymentDto
        {
            SourceAccountNumber = accountNumber,
            LoanNumber = loanNumber,
            Amount = amount,
            TellerId = userId
        };

        var result = await _transactionService.ProcessTellerLoanPaymentAsync(dto);

        if (result.IsFailure)
        {
            ViewBag.Error = result.GeneralError;
            return await Index();
        }

        ViewBag.Success = "Pago de préstamo realizado ✅";
        return await Index();
    }
}
