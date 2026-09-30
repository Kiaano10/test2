using System.Security.Claims;
using HomeAwayFromHome.API.DTOs;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = CurrentUserId;

            if (userId == null)
                return Unauthorized();

            var bookings = await _service.GetByUserAsync(userId);

            return Ok(bookings.Select(b => new
            {
                b.BookingID,
                b.PropertyID,
                PropertyName = b.Property.PropertyName,
                b.CheckInDate,
                b.CheckOutDate,
                b.NumberOfGuests,
                b.TotalAmount,
                b.Status,
                b.CreatedAt
            }));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _service.GetAllAsync();

            return Ok(bookings.Select(b => new
            {
                b.BookingID,
                b.UserID,
                b.PropertyID,
                PropertyName = b.Property.PropertyName,
                b.CheckInDate,
                b.CheckOutDate,
                b.NumberOfGuests,
                b.TotalAmount,
                b.Status,
                b.CreatedAt
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingRequest request)
        {
            var userId = CurrentUserId;

            if (userId == null)
                return Unauthorized();

            try
            {
                var booking = new Booking
                {
                    UserID = userId,
                    PropertyID = request.PropertyID,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    NumberOfGuests = request.NumberOfGuests
                };

                var created = await _service.CreateAsync(booking);

                return Created($"/api/bookings/{created.BookingID}",
                    new
                    {
                        created.BookingID,
                        created.PropertyID,
                        created.CheckInDate,
                        created.CheckOutDate,
                        created.NumberOfGuests,
                        created.TotalAmount,
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

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = CurrentUserId;

            if (userId == null)
                return Unauthorized();

            try
            {
                bool cancelled = await _service.CancelAsync(id, userId);

                return cancelled
                    ? NoContent()
                    : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}