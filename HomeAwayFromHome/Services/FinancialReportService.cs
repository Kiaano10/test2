
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Services;
using HomeAwayFromHome.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class FinancialReportService : IFinancialReportService
    {
        private readonly ApplicationDbContext _context;

        public FinancialReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MonthlyFinancialReport> GetMonthlyReportAsync(int year, int month)
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Invalid reporting period.");

            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var transactions = await _context.FinancialTransaction.AsNoTracking().Where(f => f.TransactionDate >= start && f.TransactionDate < end)
            .ToListAsync();

            return new MonthlyFinancialReport
            {
                Year = year,
                Month = month,
                TotalIncome = transactions.Where(f => f.TransactionType == "Income").Sum(f => f.Amount),
                TotalExpenses = transactions.Where(f => f.TransactionType == "Expense").Sum(f => f.Amount)
            };
        }

        public async Task<List<MonthlyFinancialReport>> GetAnnualReportAsync(
            int year)
        {
            if (year < 2000 || year > 2100)
                throw new ArgumentOutOfRangeException(nameof(year));

            var start = new DateTime(year, 1, 1);
            var end = start.AddYears(1);

            var transactions = await _context.FinancialTransaction.AsNoTracking().Where(f => f.TransactionDate >= start && f.TransactionDate < end)
            .ToListAsync();

            return Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthly = transactions.Where(f => f.TransactionDate.Month == month);

                    return new MonthlyFinancialReport
                    {
                        Year = year,
                        Month = month,
                        TotalIncome = monthly.Where(f => f.TransactionType == "Income").Sum(f => f.Amount),
                        TotalExpenses = monthly.Where(f => f.TransactionType == "Expense").Sum(f => f.Amount)
                    };
                })
                .ToList();
        }
    }
}