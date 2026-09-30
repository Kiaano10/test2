using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public interface IFinancialTransactionService
{
    Task<List<FinancialTransaction>> GetAllAsync();
    Task<FinancialTransaction?> GetByIdAsync(int id);
    Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction);
    Task<bool> UpdateAsync(int id, FinancialTransaction transaction);
    Task<bool> DeleteAsync(int id);
}
