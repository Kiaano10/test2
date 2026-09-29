
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/properties/{propertyId:int}/availability")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _service;

        public AvailabilityController(IAvailabilityService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            var records = await _service.GetByPropertyAsync(propertyId);

            return Ok(records.Select(a => new
            {
                a.AvailabilityID,
                a.PropertyID,
                a.AvailableFrom,
                a.AvailableTo,
                a.Status
            }));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int propertyId, Availability availability)
        {
            try
            {
                availability.PropertyID = propertyId;

                var created = await _service.CreateAsync(availability);

                return Created( $"/api/properties/{propertyId}/availability/{created.AvailabilityID}",
                    new
                    {
                        created.AvailabilityID,
                        created.PropertyID,
                        created.AvailableFrom,
                        created.AvailableTo,
                        created.Status
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("~/api/availability/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Availability availability)
        {
            try
            {
                bool updated = await _service.UpdateAsync(id, availability);

                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("~/api/availability/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _service.DeleteAsync(id);

            return deleted ? NoContent() : NotFound();
        }
    }
}