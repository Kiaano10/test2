using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

[Authorize(Roles = "Admin,Owner")]
public class FinancialReportsController : Controller
{
    private readonly FinancialReportApiService _api;
    public FinancialReportsController(FinancialReportApiService api) => _api = api;
    public async Task<IActionResult> Monthly(int? year, int? month) 
    { 
        var now = DateTime.Today; 
        var report = await _api.GetMonthlyAsync(year ?? now.Year, month ?? now.Month); 
        return View(report); 
    }
    public async Task<IActionResult> Annual(int? year) 
    { 
        var report = await _api.GetAnnualAsync(year ?? DateTime.Today.Year); 
        ViewBag.Year = year ?? DateTime.Today.Year; 
        return View(report); 
    }
}
