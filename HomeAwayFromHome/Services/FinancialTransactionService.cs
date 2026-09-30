using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services;

public class FinancialTransactionService : IFinancialTransactionService
{
    private readonly ApplicationDbContext _context;
    public FinancialTransactionService(ApplicationDbContext context) => _context = context;

    public async Task<List<FinancialTransaction>> GetAllAsync() =>
        await _context.FinancialTransaction.AsNoTracking().Include(x => x.Property).Include(x => x.Booking)
        .OrderByDescending(x => x.TransactionDate).ToListAsync();

    public async Task<FinancialTransaction?> GetByIdAsync(int id) =>
        await _context.FinancialTransaction.AsNoTracking().Include(x => x.Property).Include(x => x.Booking)
        .FirstOrDefaultAsync(x => x.FinancialTransactionID == id);

    public async Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction)
    {
        if (!await _context.Property.AnyAsync(p => p.PropertyID == transaction.PropertyID))
            throw new ArgumentException("The selected property does not exist.");

        if (transaction.BookingID.HasValue && !await _context.Booking.AnyAsync(b => b.BookingID == transaction.BookingID.Value))
            throw new ArgumentException("The selected booking does not exist.");

        transaction.TransactionType = transaction.TransactionType.Trim();

        if (transaction.TransactionType != "Income" && transaction.TransactionType != "Expense")
            throw new ArgumentException("Transaction type must be Income or Expense.");

        transaction.Description = transaction.Description.Trim();

        if (transaction.TransactionDate == default) transaction.TransactionDate = DateTime.UtcNow;
        _context.FinancialTransaction.Add(transaction);

        await _context.SaveChangesAsync();
        return await GetByIdAsync(transaction.FinancialTransactionID) ?? transaction;
    }

    public async Task<bool> UpdateAsync(int id, FinancialTransaction transaction)
    {
        var existing = await _context.FinancialTransaction.FirstOrDefaultAsync(x => x.FinancialTransactionID == id);
        if (existing == null) return false;

        if (!await _context.Property.AnyAsync(p => p.PropertyID == transaction.PropertyID))
            throw new ArgumentException("The selected property does not exist.");

        if (transaction.BookingID.HasValue && !await _context.Booking.AnyAsync(b => b.BookingID == transaction.BookingID.Value))
            throw new ArgumentException("The selected booking does not exist.");

        if (transaction.TransactionType != "Income" && transaction.TransactionType != "Expense")
            throw new ArgumentException("Transaction type must be Income or Expense.");

        existing.PropertyID = transaction.PropertyID; existing.BookingID = transaction.BookingID;
        existing.TransactionType = transaction.TransactionType.Trim(); existing.Description = transaction.Description.Trim();
        existing.Amount = transaction.Amount; existing.TransactionDate = transaction.TransactionDate;
        await _context.SaveChangesAsync(); return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.FinancialTransaction.FindAsync(id);

        if (existing == null) return false;

        _context.FinancialTransaction.Remove(existing); await _context.SaveChangesAsync(); return true;
    }
}
