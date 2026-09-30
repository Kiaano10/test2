using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

public class AmenitiesController : Controller
{
    private readonly AmenityApiService _api;
    public AmenitiesController(AmenityApiService api) => _api = api;
    public async Task<IActionResult> Index() => View(await _api.GetAllAsync());
    public async Task<IActionResult> Details(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }


    [Authorize(Roles = "Admin,Owner")]
    public IActionResult Create() => View(new Amenity());


    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Create(Amenity model) 
    { 
        if (!ModelState.IsValid) 
            return View(model); 

        try 
        { 
            await _api.CreateAsync(model); 
            return RedirectToAction(nameof(Index)); 
        } 
        catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            return View(model); 
        } 
    }


    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Edit(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }


    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Edit(int id, Amenity model) 
    { 
        if (id != model.AmenityID) 
            return NotFound(); 
        
        if (!ModelState.IsValid) 
            return View(model); 
        
        try 
        { 
            await _api.UpdateAsync(id, model); 
            return RedirectToAction(nameof(Index)); 
        } 
        catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            return View(model); 
        } 
    }


    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Delete(int id) 
    { 
        var x = await _api.GetByIdAsync(id); 
        return x == null ? NotFound() : View(x); 
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> DeleteConfirmed(int id) 
    { 
        try 
        { 
            await _api.DeleteAsync(id); 
            return RedirectToAction(nameof(Index)); 
        } 
        catch (HttpRequestException) 
        { 
            TempData["Error"] = "The amenity could not be deleted. It may still be assigned to a property."; 
            return RedirectToAction(nameof(Index)); 
        } 
    }
}
