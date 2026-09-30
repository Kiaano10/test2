using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

[Authorize(Roles = "Admin,Owner")]
public class FinancialTransactionsController : Controller
{
    private readonly FinancialTransactionApiService _api;
    private readonly PropertyApiService _properties;
    private readonly BookingApiService _bookings;
    public FinancialTransactionsController(FinancialTransactionApiService api, PropertyApiService properties, BookingApiService bookings) 
    { 
        _api = api; _properties = properties; 
        _bookings = bookings; 
    }
    public async Task<IActionResult> Index() => View(await _api.GetAllAsync());

    public async Task<IActionResult> Details(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }

    public async Task<IActionResult> Create() 
    { 
        ViewBag.Properties = await _properties.GetAllAsync(); 
        ViewBag.Bookings = await _bookings.GetAllAsync(); 
        return View(new FinancialTransaction { 
            TransactionDate = DateTime.Today }); 
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FinancialTransaction model) 
    { 
        if (!ModelState.IsValid) 
        { 
            await LoadLists(); 
            return View(model); 
        } 
        try 
        { 
            await _api.CreateAsync(model); 
            return RedirectToAction(nameof(Index)); 
        }
        catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            await LoadLists(); return View(model); 
        } 
    }


    public async Task<IActionResult> Edit(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 

        if (x == null) 
            return NotFound(); 

        await LoadLists(); 
        return View(x); 
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FinancialTransaction model) 
    { 
        if (id != model.FinancialTransactionID) 
            return NotFound(); 

        if (!ModelState.IsValid) 
        { 
            await LoadLists(); 
            return View(model); 
        } 
        try 
        { 
            await _api.UpdateAsync(id, model); 
            return RedirectToAction(nameof(Index)); 
        } 
        catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            await LoadLists(); 
            return View(model); 
        } 
    }

    public async Task<IActionResult> Delete(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) 
    { 
        await _api.DeleteAsync(id); 
        return RedirectToAction(nameof(Index)); 
    }

    private async Task LoadLists() 
    { 
        ViewBag.Properties = await _properties.GetAllAsync(); 
        ViewBag.Bookings = await _bookings.GetAllAsync(); 
    }
}
