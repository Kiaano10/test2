using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

[Authorize(Roles = "Admin,Owner")]
public class CustomersController : Controller
{
    private readonly CustomerApiService _api;
    public CustomersController(CustomerApiService api) => _api = api;
    public async Task<IActionResult> Index() => View(await _api.GetAllAsync());
}
