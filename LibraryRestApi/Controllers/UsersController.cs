using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryRestApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await AuthService.GetUsersAsync(FlowSide.Server);
            return Ok(all);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            // This creates a User object, not a new user in persistent storage.
            // For actual user registration, see Auth/register endpoint!
            var created = AuthService.CreateUser(user.Username, user.PasswordEncrypted, user.Role);
            return Ok(created);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? username, [FromQuery] string? role)
        {
            var allUsers = await AuthService.GetUsersAsync(FlowSide.Server);
            var results = allUsers.Data.AsQueryable();

            // Filter by username if provided
            if (!string.IsNullOrWhiteSpace(username))
            {
                results = results.Where(u =>
                    !string.IsNullOrEmpty(u.Username) &&
                    u.Username.Contains(username, System.StringComparison.OrdinalIgnoreCase)
                ).AsQueryable();
            }

            // Filter by role if provided (case-insensitive string)
            if (!string.IsNullOrWhiteSpace(role))
            {
                results = results.Where(u =>
                    !string.IsNullOrEmpty(u.Role.ToString()) &&
                    u.Role.ToString().Equals(role, System.StringComparison.OrdinalIgnoreCase)
                ).AsQueryable();
            }

            return Ok(results.ToList());
        }
    }
}
