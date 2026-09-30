using System.Net;
using System.Net.Http.Json;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public class BookingApiService
{
    private readonly HttpClient _client;
    public BookingApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");

    public async Task<List<Booking>> GetMineAsync() => await ReadBookings("api/bookings/mine");
    public async Task<List<Booking>> GetAllAsync() => await ReadBookings("api/bookings");
    private async Task<List<Booking>> ReadBookings(string url)
    {
        var rows = await _client.GetFromJsonAsync<List<BookingApiRow>>(url) ?? new();
        return rows.Select(Map).ToList();
    }
    public async Task<Booking?> GetByIdAsync(int id)
    {
        var rows = await GetMineAsync(); return rows.FirstOrDefault(x => x.BookingID == id);
    }
    public async Task<Booking?> GetAdminByIdAsync(int id)
    {
        var rows = await GetAllAsync(); return rows.FirstOrDefault(x => x.BookingID == id);
    }
    public async Task CreateAsync(Booking x)
    {
        var r = await _client.PostAsJsonAsync("api/bookings", new { x.PropertyID, x.CheckInDate, x.CheckOutDate, x.NumberOfGuests }); r.EnsureSuccessStatusCode();
    }
    public async Task CancelAsync(int id) { var r = await _client.PostAsync($"api/bookings/{id}/cancel", null); r.EnsureSuccessStatusCode(); }
    public async Task UpdateStatusAsync(int id, string status) { var r = await _client.PutAsync($"api/bookings/{id}/status?status={Uri.EscapeDataString(status)}", null); r.EnsureSuccessStatusCode(); }
    private static Booking Map(BookingApiRow x) => new() { BookingID = x.BookingID, UserID = x.UserID ?? string.Empty, PropertyID = x.PropertyID, Property = new Property { PropertyID = x.PropertyID, PropertyName = x.PropertyName ?? string.Empty }, CheckInDate = x.CheckInDate, CheckOutDate = x.CheckOutDate, NumberOfGuests = x.NumberOfGuests, TotalAmount = x.TotalAmount, Status = x.Status ?? "", CreatedAt = x.CreatedAt };
    private class BookingApiRow { public int BookingID { get; set; } public string? UserID { get; set; } public int PropertyID { get; set; } public string? PropertyName { get; set; } public DateTime CheckInDate { get; set; } public DateTime CheckOutDate { get; set; } public int NumberOfGuests { get; set; } public decimal TotalAmount { get; set; } public string? Status { get; set; } public DateTime CreatedAt { get; set; } }
}
