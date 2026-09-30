using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;

namespace HomeAwayFromHome.Services;

public class AuthApiService
{
    private readonly HttpClient _client;
    public AuthApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");

    public async Task<(bool Success, AuthApiResponse? Response, string Error)> LoginAsync(LoginApiRequest request)
    {
        var response = await _client.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode) return (false, null, await ReadError(response));
        var result = await response.Content.ReadFromJsonAsync<AuthApiResponse>();
        return result == null ? (false, null, "The API returned an empty login response.") : (true, result, "");
    }

    public async Task<(bool Success, string Error)> RegisterAsync(RegisterApiRequest request)
    {
        var response = await _client.PostAsJsonAsync("api/auth/register", request);
        return response.IsSuccessStatusCode ? (true, "") : (false, await ReadError(response));
    }

    private static async Task<string> ReadError(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body) ? $"Request failed with HTTP {(int)response.StatusCode}." : body;
    }
}
