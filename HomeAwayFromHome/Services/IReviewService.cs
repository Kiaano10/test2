
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public interface IReviewService
    {
        Task<List<Review>> GetApprovedByPropertyAsync(int propertyId);

        Task<Review?> GetByIdAsync(int id);

        Task<Review> CreateAsync(Review review);

        Task<bool> ModerateAsync(int id, string status);
    }
}