using System.Net;
using System.Net.Http.Json;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public class AmenityApiService
{
    private readonly HttpClient _client;
    public AmenityApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<List<Amenity>> GetAllAsync() => await _client.GetFromJsonAsync<List<Amenity>>("api/amenities") ?? new();
    public async Task<Amenity?> GetByIdAsync(int id) { var r = await _client.GetAsync($"api/amenities/{id}"); if (r.StatusCode == HttpStatusCode.NotFound) return null; r.EnsureSuccessStatusCode(); return await r.Content.ReadFromJsonAsync<Amenity>(); }
    public async Task CreateAsync(Amenity x) { var r = await _client.PostAsJsonAsync("api/amenities", new { x.Name, x.Description }); r.EnsureSuccessStatusCode(); }
    public async Task UpdateAsync(int id, Amenity x) { var r = await _client.PutAsJsonAsync($"api/amenities/{id}", new { x.Name, x.Description }); r.EnsureSuccessStatusCode(); }
    public async Task DeleteAsync(int id) { var r = await _client.DeleteAsync($"api/amenities/{id}"); r.EnsureSuccessStatusCode(); }
}
