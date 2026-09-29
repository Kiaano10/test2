
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;

        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            return await _context.Property.AsNoTracking().Include(p => p.PropertyAmenities)
            .ThenInclude(pa => pa.Amenity).ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Property.AsNoTracking().Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .FirstOrDefaultAsync(p => p.PropertyID == id);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            _context.Property.Add(property);
            await _context.SaveChangesAsync();

            return property;
        }

        public async Task<bool> UpdateAsync(
            int id, Property property)
        {
            var existing = await _context.Property.FirstOrDefaultAsync(p => p.PropertyID == id);

            if (existing == null)
                return false;

            existing.PropertyName = property.PropertyName;
            existing.Description = property.Description;
            existing.Address = property.Address;
            existing.MaximumGuests = property.MaximumGuests;
            existing.Bedrooms = property.Bedrooms;
            existing.Bathrooms = property.Bathrooms;
            existing.PricePerNight = property.PricePerNight;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var property = await _context.Property
                .FirstOrDefaultAsync(p => p.PropertyID == id);

            if (property == null)
                return false;

            // Prevent deletion when related records exist.
            bool hasBookings = await _context.Booking.AnyAsync(b => b.PropertyID == id);

            bool hasTransactions = await _context.FinancialTransaction.AnyAsync(f => f.PropertyID == id);

            if (hasBookings || hasTransactions)
                return false;

            _context.Property.Remove(property);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}