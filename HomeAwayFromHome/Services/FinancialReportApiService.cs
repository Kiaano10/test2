using System.Net.Http.Json;
using HomeAwayFromHome.ViewModels;

namespace HomeAwayFromHome.Services;

public class FinancialReportApiService
{
    private readonly HttpClient _client;
    public FinancialReportApiService(IHttpClientFactory factory) => _client = factory.CreateClient("HomeAwayFromHomeAPI");
    public async Task<MonthlyFinancialReport?> GetMonthlyAsync(int year, int month) => await _client.GetFromJsonAsync<MonthlyFinancialReport>($"api/financial-reports/monthly?year={year}&month={month}");
    public async Task<List<MonthlyFinancialReport>> GetAnnualAsync(int year) => await _client.GetFromJsonAsync<List<MonthlyFinancialReport>>($"api/financial-reports/annual?year={year}") ?? new();
}
