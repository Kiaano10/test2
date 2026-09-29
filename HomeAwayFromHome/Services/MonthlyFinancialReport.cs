
namespace HomeAwayFromHome.ViewModels
{
    public class MonthlyFinancialReport
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }

        public decimal Profit => TotalIncome - TotalExpenses;
    }
}