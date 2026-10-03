using HomeAwayFromHome.Data;
using HomeAwayFromHome.Models;
using HomeAwayFromHome.Services;
using Microsoft.EntityFrameworkCore;

namespace HomeAwayFromHome.Tests;

public class BookingServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task SeedAsync(ApplicationDbContext ctx)
    {
        ctx.Users.Add(new ApplicationUser { Id = "user-1", UserName = "guest@test.com" });
        ctx.Property.Add(new Property
        {
            PropertyID = 1,
            PropertyName = "Beach House",
            MaximumGuests = 4,
            PricePerNight = 1000m
        });
        await ctx.SaveChangesAsync();
    }

    private static Booking NewBooking(DateTime checkIn, DateTime checkOut, int guests = 2) => new()
    {
        UserID = "user-1",
        PropertyID = 1,
        CheckInDate = checkIn,
        CheckOutDate = checkOut,
        NumberOfGuests = guests
    };

    [Fact]
    public async Task CreateAsync_ValidBooking_CalculatesTotalAndIsPending()
    {
        using var ctx = CreateContext();
        await SeedAsync(ctx);
        var service = new BookingService(ctx);

        var result = await service.CreateAsync(
            NewBooking(new DateTime(2026, 12, 1), new DateTime(2026, 12, 4)));

        Assert.Equal(3000m, result.TotalAmount);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task CreateAsync_CheckOutBeforeCheckIn_Throws()
    {
        using var ctx = CreateContext();
        await SeedAsync(ctx);
        var service = new BookingService(ctx);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(NewBooking(new DateTime(2026, 12, 5), new DateTime(2026, 12, 1))));
    }

    [Fact]
    public async Task CreateAsync_TooManyGuests_Throws()
    {
        using var ctx = CreateContext();
        await SeedAsync(ctx);
        var service = new BookingService(ctx);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(NewBooking(new DateTime(2026, 12, 1), new DateTime(2026, 12, 3), guests: 10)));
    }

    [Fact]
    public async Task CreateAsync_OverlappingBooking_Throws()
    {
        using var ctx = CreateContext();
        await SeedAsync(ctx);
        var service = new BookingService(ctx);

        await service.CreateAsync(NewBooking(new DateTime(2026, 12, 1), new DateTime(2026, 12, 5)));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(NewBooking(new DateTime(2026, 12, 3), new DateTime(2026, 12, 7))));
    }
}
