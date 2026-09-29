
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly ApplicationDbContext _context;

        public AmenityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Amenity>> GetAllAsync()
        {
            return await _context.Amenity.AsNoTracking().OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Amenity?> GetByIdAsync(int id)
        {
            return await _context.Amenity.AsNoTracking().FirstOrDefaultAsync(a => a.AmenityID == id);
        }

        public async Task<Amenity> CreateAsync(Amenity amenity)
        {
            bool exists = await _context.Amenity.AnyAsync(a => a.Name.ToLower() == amenity.Name.ToLower());

            if (exists)
                throw new InvalidOperationException("An amenity with this name already exists.");

            _context.Amenity.Add(amenity);
            await _context.SaveChangesAsync();

            return amenity;
        }

        public async Task<bool> UpdateAsync(int id, Amenity amenity)
        {
            var existing = await _context.Amenity.FirstOrDefaultAsync(a => a.AmenityID == id);

            if (existing == null)
                return false;

            bool duplicate = await _context.Amenity.AnyAsync(a => a.AmenityID != id && a.Name.ToLower() == amenity.Name.ToLower());

            if (duplicate)
                throw new InvalidOperationException("Another amenity already uses this name.");

            existing.Name = amenity.Name;
            existing.Description = amenity.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var amenity = await _context.Amenity.Include(a => a.PropertyAmenities).FirstOrDefaultAsync(a => a.AmenityID == id);

            if (amenity == null)
                return false;

            if (amenity.PropertyAmenities.Any())
                return false;

            _context.Amenity.Remove(amenity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}