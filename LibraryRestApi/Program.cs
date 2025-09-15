using System.Text;
using LibrarySystemModels.Database;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
DataBaseService.InitLocalDb(null);
// Add services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
SessionHelperService.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
/*
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    // Get the first address, or join all
    
    var address = addresses.FirstOrDefault() ?? "notfound";
    File.WriteAllText(FileRetriever.RetrieveFIlePath("ApiBaseUrl.txt") ,address); // Save to file in working dir!

    // Optionally: Write all addresses
    // File.WriteAllText("ApiBaseUrl.txt", string.Join(Environment.NewLine, addresses));
});*/
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
