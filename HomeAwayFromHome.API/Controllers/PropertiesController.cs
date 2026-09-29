
using HomeAwayFromHome.API.DTOs;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _service;

        public PropertiesController(IPropertyService service)
        {
            _service = service;
        }

        private static PropertyResponse ToResponse(Property p)
        {
            return new PropertyResponse
            {
                PropertyID = p.PropertyID,
                PropertyName = p.PropertyName,
                Description = p.Description,
                Address = p.Address,
                MaximumGuests = p.MaximumGuests,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                PricePerNight = p.PricePerNight,
                Amenities = p.PropertyAmenities.Select(pa => pa.Amenity.Name).ToList()
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<PropertyResponse>>> GetAll()
        {
            var properties = await _service.GetAllAsync();
            return Ok(properties.Select(ToResponse).ToList());
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PropertyResponse>> GetById(int id)
        {
            var property = await _service.GetByIdAsync(id);

            if (property == null)
                return NotFound();

            return Ok(ToResponse(property));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PropertyResponse>> Create(PropertyRequest request)
        {
            var property = new Property
            {
                PropertyName = request.PropertyName.Trim(),
                Description = request.Description.Trim(),
                Address = request.Address.Trim(),
                MaximumGuests = request.MaximumGuests,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                PricePerNight = request.PricePerNight
            };

            var created = await _service.CreateAsync(property);

            return CreatedAtAction(nameof(GetById), new { id = created.PropertyID }, ToResponse(created));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, PropertyRequest request)
        {
            var property = new Property
            {
                PropertyName = request.PropertyName.Trim(),
                Description = request.Description.Trim(),
                Address = request.Address.Trim(),
                MaximumGuests = request.MaximumGuests,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                PricePerNight = request.PricePerNight
            };

            try
            {
                bool updated = await _service.UpdateAsync(id, property);
                return updated ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return Conflict(new
                    {
                        message = "Property not found or has related records."
                    });

                return NoContent();
            }
            catch (Exception ex) when (ex is Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict(new
                {
                    message = "The property cannot be deleted because it is in use."
                });
            }
        }
    }
}