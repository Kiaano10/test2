
using HomeAwayFromHome.ViewModels;

namespace HomeAwayFromHome.Services
{
    public interface IFinancialReportService
    {
        Task<MonthlyFinancialReport> GetMonthlyReportAsync(int year, int month);

        Task<List<MonthlyFinancialReport>> GetAnnualReportAsync(int year);
    }
}