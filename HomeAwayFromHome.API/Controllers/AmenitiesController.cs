using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/amenities")]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityService _service;

        public AmenitiesController(IAmenityService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var amenities = await _service.GetAllAsync();

            return Ok(amenities.Select(a => new
            {
                a.AmenityID,
                a.Name,
                a.Description
            }));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _service.GetByIdAsync(id);

            if (a == null)
                return NotFound();

            return Ok(new
            {
                a.AmenityID,
                a.Name,
                a.Description
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Amenity amenity)
        {
            try
            {
                var created = await _service.CreateAsync(new Amenity
                {
                    Name = amenity.Name.Trim(),
                    Description = amenity.Description.Trim()
                });

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.AmenityID },
                    new
                    {
                        created.AmenityID,
                        created.Name,
                        created.Description
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Amenity amenity)
        {
            try
            {
                bool updated = await _service.UpdateAsync(id, amenity);

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
            bool deleted = await _service.DeleteAsync(id);

            return deleted
                ? NoContent()
                : Conflict(new
                {
                    message = "Amenity not found or still assigned to a property."
                });
        }
    }
}