using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly BookingApiService _api;
    private readonly PropertyApiService _properties;
    public BookingsController(BookingApiService api, PropertyApiService properties) 
    { 
        _api = api; 
        _properties = properties; 
    }
    public async Task<IActionResult> Index() 
    { 
        var items = User.IsInRole("Admin") || User.IsInRole("Owner") ? await _api.GetAllAsync() : await _api.GetMineAsync(); 
        return View(items); 
    }
    public async Task<IActionResult> Details(int id) 
    { 
        var x = User.IsInRole("Admin") || User.IsInRole("Owner") ? await _api.GetAdminByIdAsync(id) : await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }
    public async Task<IActionResult> Create() 
    { 
        ViewBag.Properties = await _properties.GetAllAsync(); 
        return View(new Booking { CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1) }); 
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id) 
    { 
        await _api.CancelAsync(id); 
        return RedirectToAction(nameof(Index)); 
    }


    [Authorize(Roles = "Admin,Owner")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(int id, string status) 
    { 
        await _api.UpdateStatusAsync(id, status); 
        return RedirectToAction(nameof(Index)); 
    }
}
