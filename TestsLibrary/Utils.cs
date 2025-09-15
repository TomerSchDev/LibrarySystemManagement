using System.Security.Claims;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestsLibrary;

public static class Utils
{
    private static readonly string username = "TestAdmin";
    private static readonly string password = "123456";
    private static readonly User AdminTestUser = AuthService.CreateUser(username, password, UserRole.Admin);

    public static void LogInUser(User? user,FlowSide side)
    {
        var logUser = user ?? AdminTestUser;
        AuthService.LoginAsync(side,logUser.Username,EncryptionService.DecryptPassword(logUser)).Wait();
    }
    public static void SetFakeUser(ControllerBase controller,User? user)
    {
        var currentUser = user ?? AdminTestUser;
        LogInUser(currentUser,FlowSide.Server);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, currentUser.Username),
            new Claim(ClaimTypes.Role, currentUser.Role.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(httpContext);
        SessionHelperService.Configure(accessor.Object);

    }

    public static void setLocalTests(string dbPath)
    { 
        DataBaseService.InitLocalDb(dbPath);
        DataBaseService.SetDataServer(LocalApiSimulator.GetServer());
    }
}