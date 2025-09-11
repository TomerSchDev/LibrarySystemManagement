using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibrarySystemModels.Helpers;

namespace LibraryRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration config) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await AuthService.LoginAsync(FlowSide.Server, request.Username, request.Password);
            if (LibrarySystemModels.Models.User.IsDefaultUser(user))
            {
                var log = new LoginResponse() { Token = "", User = user };
                return Unauthorized(new ResultResolver<LoginResponse>(log,false,""));
            }

            var token = GenerateJwtToken(user);
            return Ok(new {  token, user });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = AuthService.CreateUser(request.Username, request.Password, request.Role);
            var result = await AuthService.CreateNewUserAsync(FlowSide.Server, user);
            if (!result)
                return Conflict("Registration failed (username may exist or permission denied).");

            return Ok("User successfully registered.");
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var res = await SessionHelperService.GetCurrentUser(FlowSide.Server);
            Console.WriteLine($"current user username: {res.Username} with Role permission: {res.Role}" );
            var ret = new ResultResolver<User>(res, true, "");
            return Ok(new {ret});
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public record LoginRequest(string Username, string Password);
        public record RegisterRequest(string Username, string Password, UserRole Role = UserRole.Member);
    }
}
