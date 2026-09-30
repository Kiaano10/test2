using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;

namespace HomeAwayFromHome.Services;

public class CustomerApiService
{
    private readonly HttpClient _client;
    public CustomerApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<List<CustomerResponse>> GetAllAsync() => await _client.GetFromJsonAsync<List<CustomerResponse>>("api/customers") ?? new();
}
