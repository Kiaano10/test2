using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

public class AvailabilitiesController : Controller
{
    private readonly AvailabilityApiService _api;
    private readonly PropertyApiService _properties;
    public AvailabilitiesController(AvailabilityApiService api, PropertyApiService properties) 
    { 
        _api = api; 
        _properties = properties; 
    }
    public async Task<IActionResult> Index(int? propertyId) 
    { 
        if (propertyId.HasValue) 
        { 
            ViewBag.PropertyId = propertyId; 
            return View(await _api.GetByPropertyAsync(propertyId.Value)); 
        } 
        var properties = await _properties.GetAllAsync(); 
        ViewBag.Properties = properties; 
        var all = new List<Availability>(); 
        foreach (var p in properties) 
        { 
            all.AddRange(await _api.GetByPropertyAsync(p.PropertyID)); 
        } 
        return View(all); 
    }

    [AllowAnonymous] 
    public async Task<IActionResult> Details(int id, int propertyId) 
    { 
        var x = (await _api.GetByPropertyAsync(propertyId)).FirstOrDefault(a => a.AvailabilityID == id); 
        return x == null ? NotFound() : View(x); 
    }

    [Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Create(int? propertyId) 
    { 
        ViewBag.Properties = await _properties.GetAllAsync(); 
        return View(new Availability 
        { PropertyID = propertyId ?? 0, Status = "Available" }); 
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Create(Availability model) 
    { 
        if (!ModelState.IsValid) 
        { 
            ViewBag.Properties = await _properties.GetAllAsync(); 
            return View(model); 
        } 
        try 
        { 
            await _api.CreateAsync(model); 
            return RedirectToAction(nameof(Index), new { propertyId = model.PropertyID }); 
        } 
        catch (HttpRequestException ex) 
        { 
            ModelState.AddModelError("", ex.Message); 
            ViewBag.Properties = await _properties.GetAllAsync(); 
            return View(model); 
        } 
    }


    [Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Edit(int id, int propertyId) 
    { 
        var x = (await _api.GetByPropertyAsync(propertyId)).FirstOrDefault(a => a.AvailabilityID == id); 
        return x == null ? NotFound() : View(x); 
    }


    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Edit(int id, Availability model) 
    { 
        if (id != model.AvailabilityID) 
            return NotFound(); 
        
        if (!ModelState.IsValid) 
            return View(model); 
        
        try 
        { 
            await _api.UpdateAsync(id, model); 
            return RedirectToAction(nameof(Index), new { propertyId = model.PropertyID }); 
        } 
        catch (HttpRequestException ex) 
        { ModelState.AddModelError("", ex.Message); 
            return View(model); 
        } 
    }

    [Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> Delete(int id, int propertyId) 
    { 
        var x = (await _api.GetByPropertyAsync(propertyId)).FirstOrDefault(a => a.AvailabilityID == id); 
        return x == null ? NotFound() : View(x); 
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin,Owner")] 
    public async Task<IActionResult> DeleteConfirmed(int id, int propertyId) 
    { 
        try 
        { 
            await _api.DeleteAsync(id); 
            return RedirectToAction(nameof(Index), new { propertyId }); 
        } 
        catch (HttpRequestException) 
        { return RedirectToAction(nameof(Delete), new { id, propertyId }); 
        } 
    }
}
