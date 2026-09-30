
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetApprovedByPropertyAsync(int propertyId)
        {
            return await _context.Review.AsNoTracking().Include(r => r.User).Where(r => r.Booking.PropertyID == propertyId && r.Status == "Approved")
            .OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Review.AsNoTracking().Include(r => r.User).Include(r => r.Booking).FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task<Review> CreateAsync(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var booking = await _context.Booking
                .FirstOrDefaultAsync(b => b.BookingID == review.BookingID && b.UserID == review.UserID);

            if (booking == null)
                throw new InvalidOperationException("The booking does not belong to this user.");

            if (booking.Status != "Completed" ||
                booking.CheckOutDate.Date > DateTime.UtcNow.Date)
                throw new InvalidOperationException("Only completed bookings can be reviewed.");

            bool existingReview = await _context.Review.AnyAsync(r => r.BookingID == review.BookingID);

            if (existingReview)
                throw new InvalidOperationException("This booking already has a review.");

            review.CreatedAt = DateTime.UtcNow;
            review.Status = "Pending";

            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<bool> ModerateAsync(int id, string status)
        {
            if (status != "Approved" && status != "Rejected")
                throw new ArgumentException("Status must be Approved or Rejected.");

            var review = await _context.Review.FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null)
                return false;

            review.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Review
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Booking)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}