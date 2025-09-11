using System.Net.Http.Json;
using System.Security.Claims;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using Microsoft.AspNetCore.Http;

namespace LibrarySystemModels.Services;

public static class SessionHelperService
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void Configure(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public static async Task<User> GetCurrentUser(FlowSide side)
    {
        
        if (side == FlowSide.Server)
        {
            // Standard server logic using HTTP context for JWT
            var context = _httpContextAccessor?.HttpContext;
            var principal = context?.User;
            if (principal == null) return null;
            var username = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? principal?.FindFirst("sub")?.Value
                           ?? principal?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return User.DefaultUser;
            }

            _cachedUser = await  AuthService.GetUserByUsernameAsync(FlowSide.Server, username);
            return _cachedUser;
        }

        if (!User.IsDefaultUser(_cachedUser)) return _cachedUser;
        // Client calls REST API /api/auth/current to fetch the user
        var res =await DataBaseService.Get<User>("api/Auth/Current");
        if (!res.ActionResult) return User.DefaultUser;
        _cachedUser = res.Data;
        return _cachedUser;
    }

    private static User _cachedUser = User.DefaultUser;

    // Optional: Add logic to clear _cachedUser on logout or token refresh
    public static bool IsEnoughPermission(FlowSide side, UserRole argRole)
    {
        return GetCurrentUser(side).Result.Role >= argRole;
    }
}
