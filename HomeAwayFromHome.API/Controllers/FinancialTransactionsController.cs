using HomeAwayFromHome.API.DTOs;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers;

[ApiController]
[Route("api/financial-transactions")]
[Authorize(Roles = "Admin,Owner")]
public class FinancialTransactionsController : ControllerBase
{
    private readonly IFinancialTransactionService _service;
    public FinancialTransactionsController(IFinancialTransactionService service) => _service = service;

    private static object ToResponse(FinancialTransaction x) => new { x.FinancialTransactionID, x.PropertyID, PropertyName = x.Property?.PropertyName, x.BookingID, x.TransactionType, x.Description, x.Amount, x.TransactionDate };

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok((await _service.GetAllAsync()).Select(ToResponse));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    { var x = await _service.GetByIdAsync(id); return x == null ? NotFound() : Ok(ToResponse(x)); }

    [HttpPost]
    public async Task<IActionResult> Create(FinancialTransactionRequest request)
    {
        try
        {
            var created = await _service.CreateAsync(new FinancialTransaction { PropertyID = request.PropertyID, BookingID = request.BookingID, TransactionType = request.TransactionType, Description = request.Description, Amount = request.Amount, TransactionDate = request.TransactionDate });
            return CreatedAtAction(nameof(GetById), new { id = created.FinancialTransactionID }, ToResponse(created));
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FinancialTransactionRequest request)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, new FinancialTransaction { PropertyID = request.PropertyID, BookingID = request.BookingID, TransactionType = request.TransactionType, Description = request.Description, Amount = request.Amount, TransactionDate = request.TransactionDate });
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => (await _service.DeleteAsync(id)) ? NoContent() : NotFound();
}
