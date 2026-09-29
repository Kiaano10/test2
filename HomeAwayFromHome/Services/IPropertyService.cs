
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetAllAsync();

        Task<Property?> GetByIdAsync(int id);

        Task<Property> CreateAsync(Property property);

        Task<bool> UpdateAsync(int id, Property property);

        Task<bool> DeleteAsync(int id);
    }
}