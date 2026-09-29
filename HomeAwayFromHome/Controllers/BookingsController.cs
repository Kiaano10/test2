using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // INDEX
    // =========================================================

    // GET: Bookings
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Booking
            .Include(b => b.Property)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(bookings);
    }


    // =========================================================
    // DETAILS
    // =========================================================

    // GET: Bookings/Details/5
    public async Task<IActionResult> Details(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .Include(b => b.Property)
            .Include(b => b.User)
            .FirstOrDefaultAsync(
                b => b.BookingID == bookingid
            );

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    // GET: Bookings/Create
    public IActionResult Create()
    {
        ViewData["PropertyID"] = new SelectList(
            _context.Property
                .OrderBy(p => p.PropertyName),
            "PropertyID",
            "PropertyName"
        );

        return View();
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    // POST: Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        // These values are controlled by the system,
        // not entered by the user.
        ModelState.Remove(nameof(Booking.UserID));
        ModelState.Remove(nameof(Booking.User));
        ModelState.Remove(nameof(Booking.Property));
        ModelState.Remove(nameof(Booking.TotalAmount));
        ModelState.Remove(nameof(Booking.CreatedAt));
        ModelState.Remove(nameof(Booking.Reviews));
        ModelState.Remove(nameof(Booking.FinancialTransactions));


        // -----------------------------------------------------
        // Get logged-in user
        // -----------------------------------------------------

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        booking.UserID = userId;


        // -----------------------------------------------------
        // Find property
        // -----------------------------------------------------

        var property = await _context.Property
            .FirstOrDefaultAsync(
                p => p.PropertyID == booking.PropertyID
            );

        if (property == null)
        {
            ModelState.AddModelError(
                nameof(Booking.PropertyID),
                "Please select a valid property."
            );
        }


        // -----------------------------------------------------
        // Validate dates
        // -----------------------------------------------------

        if (booking.CheckOutDate <= booking.CheckInDate)
        {
            ModelState.AddModelError(
                nameof(Booking.CheckOutDate),
                "Check-out date must be after the check-in date."
            );
        }


        // -----------------------------------------------------
        // Validate number of guests
        // -----------------------------------------------------

        if (booking.NumberOfGuests < 1)
        {
            ModelState.AddModelError(
                nameof(Booking.NumberOfGuests),
                "At least one guest is required."
            );
        }

        if (property != null &&
            booking.NumberOfGuests > property.MaximumGuests)
        {
            ModelState.AddModelError(
                nameof(Booking.NumberOfGuests),
                $"This property allows a maximum of {property.MaximumGuests} guests."
            );
        }


        // -----------------------------------------------------
        // Calculate total amount
        // -----------------------------------------------------

        if (property != null &&
            booking.CheckOutDate > booking.CheckInDate)
        {
            int numberOfNights =
                (booking.CheckOutDate - booking.CheckInDate).Days;

            booking.TotalAmount =
                property.PricePerNight * numberOfNights;
        }


        // -----------------------------------------------------
        // System-generated values
        // -----------------------------------------------------

        booking.CreatedAt = DateTime.Now;

        if (string.IsNullOrWhiteSpace(booking.Status))
        {
            booking.Status = "Pending";
        }


        // -----------------------------------------------------
        // Save
        // -----------------------------------------------------

        if (ModelState.IsValid)
        {
            _context.Booking.Add(booking);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // -----------------------------------------------------
        // Reload property dropdown if validation fails
        // -----------------------------------------------------

        ViewData["PropertyID"] = new SelectList(
            _context.Property
                .OrderBy(p => p.PropertyName),
            "PropertyID",
            "PropertyName",
            booking.PropertyID
        );

        return View(booking);
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    // GET: Bookings/Edit/5
    public async Task<IActionResult> Edit(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .Include(b => b.Property)
            .FirstOrDefaultAsync(
                b => b.BookingID == bookingid
            );

        if (booking == null)
        {
            return NotFound();
        }

        ViewData["PropertyID"] = new SelectList(
            _context.Property
                .OrderBy(p => p.PropertyName),
            "PropertyID",
            "PropertyName",
            booking.PropertyID
        );

        return View(booking);
    }


    // =========================================================
    // EDIT - POST
    // =========================================================

    // POST: Bookings/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int bookingid,
        Booking booking)
    {
        if (bookingid != booking.BookingID)
        {
            return NotFound();
        }


        // These are managed by the system.
        ModelState.Remove(nameof(Booking.UserID));
        ModelState.Remove(nameof(Booking.User));
        ModelState.Remove(nameof(Booking.Property));
        ModelState.Remove(nameof(Booking.TotalAmount));
        ModelState.Remove(nameof(Booking.CreatedAt));
        ModelState.Remove(nameof(Booking.Reviews));
        ModelState.Remove(nameof(Booking.FinancialTransactions));


        // -----------------------------------------------------
        // Get original booking
        // -----------------------------------------------------

        var existingBooking = await _context.Booking
            .AsNoTracking()
            .FirstOrDefaultAsync(
                b => b.BookingID == booking.BookingID
            );

        if (existingBooking == null)
        {
            return NotFound();
        }


        // Keep original user and creation date.
        booking.UserID = existingBooking.UserID;
        booking.CreatedAt = existingBooking.CreatedAt;


        // -----------------------------------------------------
        // Get property
        // -----------------------------------------------------

        var property = await _context.Property
            .FirstOrDefaultAsync(
                p => p.PropertyID == booking.PropertyID
            );

        if (property == null)
        {
            ModelState.AddModelError(
                nameof(Booking.PropertyID),
                "Please select a valid property."
            );
        }


        // -----------------------------------------------------
        // Validate dates
        // -----------------------------------------------------

        if (booking.CheckOutDate <= booking.CheckInDate)
        {
            ModelState.AddModelError(
                nameof(Booking.CheckOutDate),
                "Check-out date must be after the check-in date."
            );
        }


        // -----------------------------------------------------
        // Validate guests
        // -----------------------------------------------------

        if (booking.NumberOfGuests < 1)
        {
            ModelState.AddModelError(
                nameof(Booking.NumberOfGuests),
                "At least one guest is required."
            );
        }

        if (property != null &&
            booking.NumberOfGuests > property.MaximumGuests)
        {
            ModelState.AddModelError(
                nameof(Booking.NumberOfGuests),
                $"This property allows a maximum of {property.MaximumGuests} guests."
            );
        }


        // -----------------------------------------------------
        // Recalculate amount
        // -----------------------------------------------------

        if (property != null &&
            booking.CheckOutDate > booking.CheckInDate)
        {
            int numberOfNights =
                (booking.CheckOutDate - booking.CheckInDate).Days;

            booking.TotalAmount =
                property.PricePerNight * numberOfNights;
        }


        // -----------------------------------------------------
        // Save changes
        // -----------------------------------------------------

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingID))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // Reload dropdown if validation fails.

        ViewData["PropertyID"] = new SelectList(
            _context.Property
                .OrderBy(p => p.PropertyName),
            "PropertyID",
            "PropertyName",
            booking.PropertyID
        );

        return View(booking);
    }


    // =========================================================
    // DELETE - GET
    // =========================================================

    // GET: Bookings/Delete/5
    public async Task<IActionResult> Delete(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .Include(b => b.Property)
            .Include(b => b.User)
            .FirstOrDefaultAsync(
                b => b.BookingID == bookingid
            );

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }


    // =========================================================
    // DELETE - POST
    // =========================================================

    // POST: Bookings/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int bookingid)
    {
        var booking = await _context.Booking
            .FindAsync(bookingid);

        if (booking != null)
        {
            _context.Booking.Remove(booking);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // HELPER
    // =========================================================

    private bool BookingExists(int bookingid)
    {
        return _context.Booking
            .Any(e => e.BookingID == bookingid);
    }
}