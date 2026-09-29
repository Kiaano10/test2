using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;

namespace HomeAwayFromHome.Services
{
    public class PropertyApiService
    {
        private readonly HttpClient _httpClient;

        public PropertyApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("HomeAwayFromHomeAPI");
        }

        public async Task<List<PropertyResponse>> GetAllAsync()
        {
            var properties = await _httpClient.GetFromJsonAsync<List<PropertyResponse>>("api/properties");

            return properties ?? new List<PropertyResponse>();
        }

        public async Task<PropertyResponse?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PropertyResponse>($"api/properties/{id}");
        }
    }
}