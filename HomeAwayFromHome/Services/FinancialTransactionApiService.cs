using System.Net;
using System.Net.Http.Json;
using HomeAwayFromHome.DTOs;
using HomeAwayFromHome.Models;

namespace HomeAwayFromHome.Services;

public class FinancialTransactionApiService
{
    private readonly HttpClient _client;
    public FinancialTransactionApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<List<FinancialTransaction>> GetAllAsync() { var r = await _client.GetFromJsonAsync<List<Row>>("api/financial-transactions") ?? new(); return r.Select(Map).ToList(); }
    public async Task<FinancialTransaction?> GetByIdAsync(int id) { var r = await _client.GetAsync($"api/financial-transactions/{id}"); if (r.StatusCode == HttpStatusCode.NotFound) return null; r.EnsureSuccessStatusCode(); var x = await r.Content.ReadFromJsonAsync<Row>(); return x == null ? null : Map(x); }
    public async Task CreateAsync(FinancialTransaction x) { var r = await _client.PostAsJsonAsync("api/financial-transactions", Request(x)); r.EnsureSuccessStatusCode(); }
    public async Task UpdateAsync(int id, FinancialTransaction x) { var r = await _client.PutAsJsonAsync($"api/financial-transactions/{id}", Request(x)); r.EnsureSuccessStatusCode(); }
    public async Task DeleteAsync(int id) { var r = await _client.DeleteAsync($"api/financial-transactions/{id}"); r.EnsureSuccessStatusCode(); }
    private static FinancialTransactionApiRequest Request(FinancialTransaction x) => new() { PropertyID = x.PropertyID, BookingID = x.BookingID, TransactionType = x.TransactionType, Description = x.Description, Amount = x.Amount, TransactionDate = x.TransactionDate };
    private static FinancialTransaction Map(Row x) => new() { FinancialTransactionID = x.FinancialTransactionID, PropertyID = x.PropertyID, Property = new Property { PropertyID = x.PropertyID, PropertyName = x.PropertyName ?? string.Empty }, BookingID = x.BookingID, TransactionType = x.TransactionType ?? string.Empty, Description = x.Description ?? string.Empty, Amount = x.Amount, TransactionDate = x.TransactionDate };
    private class Row { public int FinancialTransactionID { get; set; } public int PropertyID { get; set; } public string? PropertyName { get; set; } public int? BookingID { get; set; } public string? TransactionType { get; set; } public string? Description { get; set; } public decimal Amount { get; set; } public DateTime TransactionDate { get; set; } }
}
