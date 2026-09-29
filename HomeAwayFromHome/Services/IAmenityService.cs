
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public interface IAmenityService
    {
        Task<List<Amenity>> GetAllAsync();

        Task<Amenity?> GetByIdAsync(int id);

        Task<Amenity> CreateAsync(Amenity amenity);

        Task<bool> UpdateAsync(int id, Amenity amenity);

        Task<bool> DeleteAsync(int id);
    }
}