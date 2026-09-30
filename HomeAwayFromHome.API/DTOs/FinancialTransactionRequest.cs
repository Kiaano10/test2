using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.API.DTOs;

public class FinancialTransactionRequest
{
    [Range(1, int.MaxValue)] public int PropertyID { get; set; }
    public int? BookingID { get; set; }
    [Required, StringLength(20)] public string TransactionType { get; set; } = string.Empty;
    [Required, StringLength(250)] public string Description { get; set; } = string.Empty;
    [Range(0.01, 10000000, ErrorMessage = "Amount must be between R0.01 and R10,000,000.")]
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}
