
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Booking.AsNoTracking().Include(b => b.Property).OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        }

        public async Task<List<Booking>> GetByUserAsync(string userId)
        {
            return await _context.Booking.AsNoTracking().Include(b => b.Property).Where(b => b.UserID == userId).OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Booking .AsNoTracking().Include(b => b.Property).Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BookingID == id);
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            var property = await _context.Property.FirstOrDefaultAsync(p => p.PropertyID == booking.PropertyID);

            if (property == null)
                throw new InvalidOperationException("Property does not exist.");

            if (booking.CheckInDate.Date >= booking.CheckOutDate.Date)
                throw new ArgumentException("Check-out must be after check-in.");

            if (booking.NumberOfGuests < 1 ||
                booking.NumberOfGuests > property.MaximumGuests)
                throw new ArgumentException("The guest count exceeds the property's capacity.");

            bool userExists = await _context.Users.AnyAsync(u => u.Id == booking.UserID);

            if (!userExists)
                throw new InvalidOperationException("The specified user does not exist.");

            var checkIn = booking.CheckInDate.Date;
            var checkOut = booking.CheckOutDate.Date;

            bool unavailable = await _context.Availability.AnyAsync(a => a.PropertyID == booking.PropertyID && a.Status == "Unavailable" && a.AvailableFrom < checkOut &&
            checkIn < a.AvailableTo);

            if (unavailable)
                throw new InvalidOperationException("The property is unavailable for those dates.");

            // When availability records exist, require the requested nights to be explicitly available.
            bool hasAvailabilityRecords = await _context.Availability.AnyAsync(a => a.PropertyID == booking.PropertyID);

            if (hasAvailabilityRecords)
            {
                for (var night = checkIn;
                     night < checkOut;
                     night = night.AddDays(1))
                {
                    bool available = await _context.Availability.AnyAsync(a => a.PropertyID == booking.PropertyID && a.Status == "Available" &&
                        a.AvailableFrom <= night && a.AvailableTo > night);

                    if (!available)
                        throw new InvalidOperationException($"The property is not marked available on {night:yyyy-MM-dd}.");
                }
            }

            bool conflictingBooking = await _context.Booking.AnyAsync(b => b.PropertyID == booking.PropertyID && (b.Status == "Pending" || b.Status == "Confirmed") &&
            b.CheckInDate < checkOut && checkIn < b.CheckOutDate);

            if (conflictingBooking)
                throw new InvalidOperationException("The property is already booked for those dates.");

            int nights = (checkOut - checkIn).Days;

            booking.CheckInDate = checkIn;
            booking.CheckOutDate = checkOut;
            booking.TotalAmount = nights * property.PricePerNight;
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.UtcNow;

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<bool> CancelAsync(int id, string userId)
        {
            var booking = await _context.Booking.FirstOrDefaultAsync(b => b.BookingID == id &&
             b.UserID == userId);

            if (booking == null)
                return false;

            if (booking.Status != "Pending" && booking.Status != "Confirmed")
                throw new InvalidOperationException("This booking cannot be cancelled.");

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}