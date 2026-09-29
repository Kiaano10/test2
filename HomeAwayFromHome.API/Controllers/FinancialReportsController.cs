
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers
{
    [ApiController]
    [Route("api/financial-reports")]
    [Authorize(Roles = "Admin")]
    public class FinancialReportsController : ControllerBase
    {
        private readonly IFinancialReportService _service;

        public FinancialReportsController(IFinancialReportService service)
        {
            _service = service;
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> Monthly([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var report = await _service.GetMonthlyReportAsync(year, month);

                return Ok(report);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("annual")]
        public async Task<IActionResult> Annual([FromQuery] int year)
        {
            try
            {
                var report = await _service.GetAnnualReportAsync(year);
                return Ok(report);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}