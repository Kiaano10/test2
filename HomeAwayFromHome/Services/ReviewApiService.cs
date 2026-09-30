using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public class ReviewApiService
{
    private readonly HttpClient _client;
    public ReviewApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<List<Review>> GetByPropertyAsync(int propertyId) { var r = await _client.GetFromJsonAsync<List<ReviewApiRow>>($"api/reviews/property/{propertyId}") ?? new(); return r.Select(Map).ToList(); }
    public async Task<List<Review>> GetAllAsync() { var r = await _client.GetFromJsonAsync<List<ReviewApiRow>>("api/reviews/all") ?? new(); return r.Select(Map).ToList(); }
    public async Task CreateAsync(Review x) { var r = await _client.PostAsJsonAsync("api/reviews", new ReviewApiRequest { BookingID = x.BookingID, Rating = x.Rating, Comment = x.Comment }); r.EnsureSuccessStatusCode(); }
    public async Task ModerateAsync(int id, string status) { var r = await _client.PutAsync($"api/reviews/{id}/moderate?status={Uri.EscapeDataString(status)}", null); r.EnsureSuccessStatusCode(); }
    private static Review Map(ReviewApiRow x) => new() { ReviewID = x.ReviewID, UserID = x.UserID ?? string.Empty, BookingID = x.BookingID, Rating = x.Rating, Comment = x.Comment ?? string.Empty, Status = x.Status ?? "Approved", CreatedAt = x.CreatedAt };
    private class ReviewApiRow { public int ReviewID { get; set; } public string? UserID { get; set; } public int BookingID { get; set; } public string? Reviewer { get; set; } public int Rating { get; set; } public string? Comment { get; set; } public string? Status { get; set; } public DateTime CreatedAt { get; set; } }
}
