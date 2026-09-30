using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

[Authorize(Roles = "Admin,Owner")]
public class DashboardController : Controller
{
    private readonly PropertyApiService _properties;
    private readonly BookingApiService _bookings;
    private readonly FinancialTransactionApiService _transactions;
    private readonly CustomerApiService _customers;

    public DashboardController(PropertyApiService properties, BookingApiService bookings, FinancialTransactionApiService transactions, CustomerApiService customers)
    {
        _properties = properties;
        _bookings = bookings;
        _transactions = transactions;
        _customers = customers;
    }

    public async Task<IActionResult> Index()
    {
        var properties = await _properties.GetAllAsync();
        var bookings = await _bookings.GetAllAsync();
        var transactions = await _transactions.GetAllAsync();
        var customers = await _customers.GetAllAsync();
        var month = DateTime.Today;
        var monthTransactions = transactions.Where(x => x.TransactionDate.Year == month.Year && x.TransactionDate.Month == month.Month).ToList();

        ViewBag.PropertyCount = properties.Count;
        ViewBag.BookingCount = bookings.Count;
        ViewBag.PendingBookings = bookings.Count(x => x.Status == "Pending");
        ViewBag.CustomerCount = customers.Count;
        ViewBag.MonthIncome = monthTransactions.Where(x => x.TransactionType == "Income").Sum(x => x.Amount);
        ViewBag.MonthExpenses = monthTransactions.Where(x => x.TransactionType == "Expense").Sum(x => x.Amount);
        ViewBag.MonthProfit = (decimal)ViewBag.MonthIncome - (decimal)ViewBag.MonthExpenses;
        return View();
    }
}
