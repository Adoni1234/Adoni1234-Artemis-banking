using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = "Client")]
public class BeneficiaryController : Controller
{
    private readonly IBeneficiaryService _beneficiaryService;

    public BeneficiaryController(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var list = await _beneficiaryService.GetByUserIdAsync(userId);

        return View(list);
    }

    // ================= CREATE =================
    [HttpPost]
    public async Task<IActionResult> Create(string accountNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var dto = new BeneficiaryDto
        {
            UserId = userId,
            SavingAccountId = accountNumber
        };

        await _beneficiaryService.AddAsync(dto);

        return RedirectToAction("Index");
    }

    // ================= DELETE =================
    public async Task<IActionResult> Delete(int id)
    {
        await _beneficiaryService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}
