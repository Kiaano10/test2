
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Availability>> GetByPropertyAsync(int propertyId)
        {
            return await _context.Availability.AsNoTracking().Where(a => a.PropertyID == propertyId)
            .OrderBy(a => a.AvailableFrom).ToListAsync();
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            return await _context.Availability.AsNoTracking().FirstOrDefaultAsync(a => a.AvailabilityID == id);
        }

        public async Task<Availability> CreateAsync(Availability availability)
        {
            ValidateDates(availability.AvailableFrom, availability.AvailableTo);

            bool propertyExists = await _context.Property.AnyAsync(p => p.PropertyID == availability.PropertyID);

            if (!propertyExists)
                throw new InvalidOperationException("Property does not exist.");

            bool overlap = await _context.Availability.AnyAsync(a => a.PropertyID == availability.PropertyID &&
            a.AvailableFrom < availability.AvailableTo && availability.AvailableFrom < a.AvailableTo);

            if (overlap)
                throw new InvalidOperationException("This availability period overlaps an existing period.");

            availability.Status = NormalizeStatus(availability.Status);

            _context.Availability.Add(availability);
            await _context.SaveChangesAsync();

            return availability;
        }

        public async Task<bool> UpdateAsync(int id, Availability availability)
        {
            ValidateDates(availability.AvailableFrom, availability.AvailableTo);

            var existing = await _context.Availability.FirstOrDefaultAsync(a => a.AvailabilityID == id);

            if (existing == null)
                return false;

            bool overlap = await _context.Availability.AnyAsync(a => a.AvailabilityID != id && a.PropertyID == existing.PropertyID &&
            a.AvailableFrom < availability.AvailableTo && availability.AvailableFrom < a.AvailableTo);

            if (overlap)
                throw new InvalidOperationException(
                    "This availability period overlaps an existing period.");

            existing.AvailableFrom = availability.AvailableFrom;
            existing.AvailableTo = availability.AvailableTo;
            existing.Status = NormalizeStatus(availability.Status);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var availability = await _context.Availability.FirstOrDefaultAsync(a => a.AvailabilityID == id);

            if (availability == null)
                return false;

            _context.Availability.Remove(availability);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void ValidateDates(DateTime from, DateTime to)
        {
            if (from.Date >= to.Date)
                throw new ArgumentException("The end date must be after the start date.");
        }

        private static string NormalizeStatus(string status)
        {
            if (string.Equals(status, "Available", StringComparison.OrdinalIgnoreCase))
                return "Available";

            if (string.Equals(status, "Unavailable", StringComparison.OrdinalIgnoreCase))
                return "Unavailable";

            throw new ArgumentException("Status must be Available or Unavailable.");
        }
    }
}