namespace HomeAwayFromHome.DTOs;

public class FinancialTransactionApiRequest
{
    public int PropertyID { get; set; }
    public int? BookingID { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}
