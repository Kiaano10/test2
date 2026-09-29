
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();

        Task<List<Booking>> GetByUserAsync(string userId);

        Task<Booking?> GetByIdAsync(int id);

        Task<Booking> CreateAsync(Booking booking);

        Task<bool> CancelAsync(int id, string userId);
    }
}