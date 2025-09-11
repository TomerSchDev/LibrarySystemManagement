
using LibrarySystemModels.Services;

namespace LibrarySystemModels.Models;

public enum UserRole
{
    Member = 0,
    Librarian = 1,
    Admin = 2,
}
public record LoginResponse
{
    public string Token { get; set; }
    public User User { get; set; }
}
public class User : IExportable
{
    public static readonly User DefaultUser = AuthService.CreateUser("Default User","password", UserRole.Admin);
    public User() {}

    // On registration or password reset, use this constructor
    public User(string username, string password,string passwordHash,string passwordSalt ,UserRole role = UserRole.Member )
    {
        Username = username;
        Role = role;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        PasswordEncrypted=password;
    }

    public int UserID { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
    public string UserRoleToString => Role.ToString();
    public string ExportClassName => "User";

    // --- Security fields ---
    public string PasswordHash { get; set; }        // For authentication only
    public string PasswordSalt { get; set; }        // Unique salt
    public string PasswordEncrypted { get; set; }   // For admin recovery/display
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(User)) return false;
        var u = (User)obj;
        return Equals(u);
    }

    private bool Equals(User other)
    {
        return UserID == other.UserID && Username == other.Username && Role == other.Role && PasswordHash == other.PasswordHash && PasswordSalt == other.PasswordSalt && PasswordEncrypted == other.PasswordEncrypted;
    }
    public static bool IsDefaultUser(User user) => DefaultUser.Equals(user);
   
    
}