
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public interface IAvailabilityService
    {
        Task<List<Availability>> GetByPropertyAsync(int propertyId);

        Task<Availability?> GetByIdAsync(int id);

        Task<Availability> CreateAsync(Availability availability);

        Task<bool> UpdateAsync(int id, Availability availability);

        Task<bool> DeleteAsync(int id);
    }
}