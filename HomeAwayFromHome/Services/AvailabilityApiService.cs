using System.Net;
using System.Net.Http.Json;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public class AvailabilityApiService
{
    private readonly HttpClient _client;
    public AvailabilityApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<List<Availability>> GetByPropertyAsync(int propertyId) => await _client.GetFromJsonAsync<List<Availability>>($"api/properties/{propertyId}/availability") ?? new();
    public async Task<Availability?> GetByIdAsync(int id)
    {
        // The API currently exposes availability GET by property, so MVC finds it from the property collection.
        throw new NotSupportedException("Use GetByPropertyAsync because the API does not expose a standalone GET by availability id.");
    }
    public async Task CreateAsync(Availability x) { var r = await _client.PostAsJsonAsync($"api/properties/{x.PropertyID}/availability", new { x.AvailableFrom, x.AvailableTo, x.Status }); r.EnsureSuccessStatusCode(); }
    public async Task UpdateAsync(int id, Availability x) { var r = await _client.PutAsJsonAsync($"api/availability/{id}", new { x.PropertyID, x.AvailableFrom, x.AvailableTo, x.Status }); r.EnsureSuccessStatusCode(); }
    public async Task DeleteAsync(int id) { var r = await _client.DeleteAsync($"api/availability/{id}"); r.EnsureSuccessStatusCode(); }
}
