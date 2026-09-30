using System.Security.Claims;
using HomeAwayFromHome.API.DTOs;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("property/{propertyId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            var reviews = await _service.GetApprovedByPropertyAsync(propertyId);

            return Ok(reviews.Select(r => new
            {
                r.ReviewID,
                r.BookingID,
                Reviewer = $"{r.User.FirstName} {r.User.LastName}",
                r.Rating,
                r.Comment,
                r.CreatedAt
            }));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            try
            {
                var review = new Review
                {
                    UserID = userId,
                    BookingID = request.BookingID,
                    Rating = request.Rating,
                    Comment = request.Comment.Trim()
                };

                var created = await _service.CreateAsync(review);

                return Created($"/api/reviews/{created.ReviewID}",
                    new
                    {
                        created.ReviewID,
                        created.BookingID,
                        created.Rating,
                        created.Comment,
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

        [HttpPut("{id:int}/moderate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Moderate(int id, [FromQuery] string status)
        {
            try
            {
                bool updated = await _service.ModerateAsync(id, status);

                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}