using System.Net;
using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services
{
    public class PropertyApiService
    {
        private readonly HttpClient _httpClient;

        public PropertyApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(
                "HomeAwayFromHomeAPI");
        }

        public async Task<List<Property>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<
                List<PropertyResponse>>("api/properties");

            return response?.Select(MapToProperty).ToList()
                   ?? new List<Property>();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(
                $"api/properties/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var property = await response.Content
                .ReadFromJsonAsync<PropertyResponse>();

            return property == null ? null : MapToProperty(property);
        }

        public async Task CreateAsync(Property property)
        {
            var request = new PropertyApiRequest
            {
                PropertyName = property.PropertyName,
                Description = property.Description,
                Address = property.Address,
                MaximumGuests = property.MaximumGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                PricePerNight = property.PricePerNight
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/properties", request);

            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, Property property)
        {
            var request = new PropertyApiRequest
            {
                PropertyName = property.PropertyName,
                Description = property.Description,
                Address = property.Address,
                MaximumGuests = property.MaximumGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                PricePerNight = property.PricePerNight
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"api/properties/{id}", request);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/properties/{id}");

            response.EnsureSuccessStatusCode();
        }

        private static Property MapToProperty(
            PropertyResponse response)
        {
            return new Property
            {
                PropertyID = response.PropertyID,
                PropertyName = response.PropertyName,
                Description = response.Description,
                Address = response.Address,
                MaximumGuests = response.MaximumGuests,
                Bedrooms = response.Bedrooms,
                Bathrooms = response.Bathrooms,
                PricePerNight = response.PricePerNight
            };
        }
    }
}