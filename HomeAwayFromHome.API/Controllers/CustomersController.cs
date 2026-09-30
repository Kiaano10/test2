using HomeAwayFromHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin,Owner")]
public class CustomersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    public CustomersController(UserManager<ApplicationUser> users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _users.GetUsersInRoleAsync("Customer");
        return Ok(users.Select(u => new { UserID = u.Id, u.FirstName, u.LastName, Email = u.Email ?? "" }));
    }
}
