using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeAwayFromHome.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly BookingApiService _api;
    private readonly PropertyApiService _properties;

    public BookingsController(
        BookingApiService api,
        PropertyApiService properties)
    {
        _api = api;
        _properties = properties;
    }


    // =========================================================
    // INDEX
    // =========================================================

    public async Task<IActionResult> Index()
    {
        var items =
            User.IsInRole("Admin") || User.IsInRole("Owner")
                ? await _api.GetAllAsync()
                : await _api.GetMineAsync();

        return View(items);
    }


    // =========================================================
    // DETAILS
    // =========================================================

    public async Task<IActionResult> Details(int id)
    {
        var booking =
            User.IsInRole("Admin") || User.IsInRole("Owner")
                ? await _api.GetAdminByIdAsync(id)
                : await _api.GetByIdAsync(id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    public async Task<IActionResult> Create()
    {
        await LoadPropertyDropdown();

        var booking = new Booking
        {
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 1
        };

        return View(booking);
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        // These properties are controlled by the API/server.
        ModelState.Remove(nameof(Booking.UserID));
        ModelState.Remove(nameof(Booking.User));
        ModelState.Remove(nameof(Booking.Property));
        ModelState.Remove(nameof(Booking.TotalAmount));
        ModelState.Remove(nameof(Booking.Status));
        ModelState.Remove(nameof(Booking.CreatedAt));
        ModelState.Remove(nameof(Booking.Reviews));
        ModelState.Remove(nameof(Booking.FinancialTransactions));


        // Validate property
        if (booking.PropertyID <= 0)
        {
            ModelState.AddModelError(
                nameof(Booking.PropertyID),
                "Please select a property.");
        }


        // Validate dates
        if (booking.CheckOutDate <= booking.CheckInDate)
        {
            ModelState.AddModelError(
                nameof(Booking.CheckOutDate),
                "Check-out date must be after the check-in date.");
        }


        // Validate guests
        if (booking.NumberOfGuests < 1)
        {
            ModelState.AddModelError(
                nameof(Booking.NumberOfGuests),
                "At least one guest is required.");
        }


        if (!ModelState.IsValid)
        {
            await LoadPropertyDropdown(booking.PropertyID);
            return View(booking);
        }


        try
        {
            await _api.CreateAsync(booking);

            TempData["Success"] =
                "Your booking was created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(
                "",
                $"Unable to create the booking. {ex.Message}");

            await LoadPropertyDropdown(booking.PropertyID);

            return View(booking);
        }
    }


    // =========================================================
    // CANCEL BOOKING
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _api.CancelAsync(id);

            TempData["Success"] =
                "The booking was cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] =
                $"Unable to cancel the booking. {ex.Message}";

            return RedirectToAction(nameof(Index));
        }
    }


    // =========================================================
    // UPDATE BOOKING STATUS
    // =========================================================

    [Authorize(Roles = "Admin,Owner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(
        int id,
        string status)
    {
        try
        {
            await _api.UpdateStatusAsync(id, status);

            TempData["Success"] =
                "The booking status was updated.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] =
                $"Unable to update the booking status. {ex.Message}";

            return RedirectToAction(nameof(Index));
        }
    }


    // =========================================================
    // PROPERTY DROPDOWN
    // =========================================================

    private async Task LoadPropertyDropdown(
        int? selectedPropertyId = null)
    {
        try
        {
            var properties =
                await _properties.GetAllAsync();

            ViewBag.PropertyID = new SelectList(
                properties,
                "PropertyID",
                "PropertyName",
                selectedPropertyId
            );
        }
        catch (HttpRequestException)
        {
            ViewBag.PropertyID =
                new SelectList(
                    new List<Property>(),
                    "PropertyID",
                    "PropertyName"
                );

            ModelState.AddModelError(
                "",
                "Unable to load the available properties.");
        }
    }
}