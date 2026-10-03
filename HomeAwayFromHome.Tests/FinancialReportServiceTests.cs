using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Tests;

public class FinancialReportServiceTests
{
    [Fact]
    public async Task GetMonthlyReportAsync_CalculatesProfit()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var ctx = new ApplicationDbContext(options);

        ctx.FinancialTransaction.AddRange(
            new FinancialTransaction { PropertyID = 1, TransactionType = "Income", Amount = 5000m, TransactionDate = new DateTime(2026, 11, 5) },
            new FinancialTransaction { PropertyID = 1, TransactionType = "Expense", Amount = 1200m, TransactionDate = new DateTime(2026, 11, 20) },
            new FinancialTransaction { PropertyID = 1, TransactionType = "Income", Amount = 9999m, TransactionDate = new DateTime(2026, 12, 1) });
        await ctx.SaveChangesAsync();

        var report = await new FinancialReportService(ctx).GetMonthlyReportAsync(2026, 11);

        Assert.Equal(5000m, report.TotalIncome);
        Assert.Equal(1200m, report.TotalExpenses);
        Assert.Equal(3800m, report.Profit);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_InvalidMonth_Throws()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var ctx = new ApplicationDbContext(options);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            new FinancialReportService(ctx).GetMonthlyReportAsync(2026, 13));
    }
}
