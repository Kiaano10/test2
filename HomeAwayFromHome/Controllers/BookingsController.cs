
using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: BOOKINGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Booking.ToListAsync());
    }

    // GET: BOOKINGS/Details/5
    public async Task<IActionResult> Details(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .FirstOrDefaultAsync(m => m.BookingID == bookingid);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }
    // GET: Bookings/Create
    public IActionResult Create()
    {
        ViewData["PropertyID"] = new SelectList(
            _context.Property,
            "PropertyID",
            "PropertyName"
        );

        return View();
    }


    // POST: Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        if (ModelState.IsValid)
        {
            booking.CreatedAt = DateTime.Now;

            _context.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewData["PropertyID"] = new SelectList(
            _context.Property,
            "PropertyID",
            "PropertyName",
            booking.PropertyID
        );

        return View(booking);
    }

    // GET: BOOKINGS/Edit/5
    public async Task<IActionResult> Edit(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking.FindAsync(bookingid);
        if (booking == null)
        {
            return NotFound();
        }
        return View(booking);
    }

    // POST: BOOKINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? bookingid, [Bind("BookingID,UserID,User,PropertyID,Property,CheckInDate,CheckOutDate,NumberOfGuests,TotalAmount,Status,CreatedAt,Reviews,FinancialTransactions")] Booking booking)
    {
        if (bookingid != booking.BookingID)
        {
            return NotFound();
        }

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
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(booking);
    }

    // GET: BOOKINGS/Delete/5
    public async Task<IActionResult> Delete(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .FirstOrDefaultAsync(m => m.BookingID == bookingid);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // POST: BOOKINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? bookingid)
    {
        var booking = await _context.Booking.FindAsync(bookingid);
        if (booking != null)
        {
            _context.Booking.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int? bookingid)
    {
        return _context.Booking.Any(e => e.BookingID == bookingid);
    }
}
