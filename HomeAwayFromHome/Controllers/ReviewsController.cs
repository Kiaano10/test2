using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

public class ReviewsController : Controller
{
    private readonly ReviewApiService _api;
    private readonly PropertyApiService _properties;
    private readonly BookingApiService _bookings;
    public ReviewsController(ReviewApiService api, PropertyApiService properties, BookingApiService bookings) 
    { 
        _api = api; _properties = properties; 
        _bookings = bookings; 
    }

    [Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Index() 
    { 
        return View(await _api.GetAllAsync()); 
    }


    [AllowAnonymous] 
    public async Task<IActionResult> Property(int propertyId) 
    { 
        ViewBag.PropertyId = propertyId; 
        return View("Index", await _api.GetByPropertyAsync(propertyId)); 
    }


    [Authorize] 
    public async Task<IActionResult> Create() 
    { 
        ViewBag.Bookings = (await _bookings.GetMineAsync()).Where(b => b.Status == "Completed").ToList(); 
        return View(new Review()); 
    }


    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Create(Review model) 
    {
        if (!ModelState.IsValid) 
        { 
            ViewBag.Bookings = (await _bookings.GetMineAsync()).Where(b => b.Status == "Completed").ToList(); 
            return View(model); 
        }
        try 
        { 
            await _api.CreateAsync(model); 
            TempData["Success"] = "Review submitted for moderation."; 
            return RedirectToAction("Index", "Home"); 
        } catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            ViewBag.Bookings = (await _bookings.GetMineAsync()).Where(b => b.Status == "Completed").ToList(); 
            return View(model); 
        } 
    }

    [Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Details(int id) 
    { 
        var x = (await _api.GetAllAsync()).FirstOrDefault(r => r.ReviewID == id); return x == null ? NotFound() : View(x); 
    }


    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Moderate(int id, string status) 
    { 
        await _api.ModerateAsync(id, status); return RedirectToAction(nameof(Index)); 
    }
}
