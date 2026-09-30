using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Mvc;

public class PropertiesController : Controller
{
    private readonly PropertyApiService _propertyApi;

    public PropertiesController(PropertyApiService propertyApi)
    {
        _propertyApi = propertyApi;
    }

    // GET: Properties
    public async Task<IActionResult> Index()
    {
        try
        {
            var properties = await _propertyApi.GetAllAsync();
            return View(properties);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] =
                "Unable to retrieve properties from the API.";
            return View(new List<Property>());
        }
    }

    // GET: Properties/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var property = await _propertyApi.GetByIdAsync(id);

            if (property == null)
                return NotFound();

            return View(property);
        }
        catch (HttpRequestException)
        {
            return Problem(
                "Unable to retrieve the property from the API.");
        }
    }

    // GET: Properties/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Properties/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("PropertyName,Description,Address,MaximumGuests," +
              "Bedrooms,Bathrooms,PricePerNight")]
        Property property)
    {
        if (!ModelState.IsValid)
            return View(property);

        try
        {
            await _propertyApi.CreateAsync(property);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("",
                "Unable to create the property. " +
                "Check the API and your permissions.");
            return View(property);
        }
    }

    // GET: Properties/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var property = await _propertyApi.GetByIdAsync(id);

            if (property == null)
                return NotFound();

            return View(property);
        }
        catch (HttpRequestException)
        {
            return Problem(
                "Unable to retrieve the property from the API.");
        }
    }

    // POST: Properties/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("PropertyID,PropertyName,Description,Address," +
              "MaximumGuests,Bedrooms,Bathrooms,PricePerNight")]
        Property property)
    {
        if (id != property.PropertyID)
            return NotFound();

        if (!ModelState.IsValid)
            return View(property);

        try
        {
            await _propertyApi.UpdateAsync(id, property);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("",
                "Unable to update the property. " +
                "Check the API and your permissions.");
            return View(property);
        }
    }

    // GET: Properties/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var property = await _propertyApi.GetByIdAsync(id);

            if (property == null)
                return NotFound();

            return View(property);
        }
        catch (HttpRequestException)
        {
            return Problem(
                "Unable to retrieve the property from the API.");
        }
    }

    // POST: Properties/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _propertyApi.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Error"] =
                "Unable to delete the property. " +
                "It may have existing bookings or you may lack permission.";

            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}