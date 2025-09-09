using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Models;

public enum UserRole
{
    Member = 0,
    Librarian = 1,
    Admin = 2,
}

public class User : IExportable
{
    public User() {}

    // On registration or password reset, use this constructor
    public User(string username, string password, UserRole role = UserRole.Member)
    {
        Username = username;
        Role = role;
        SetPassword(password);
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

    // Set or reset password (hash+salt/encrypt)
    public void SetPassword(string password)
    {
        var (hash, salt) = EncryptionService.CreateHash(password);
        PasswordHash = hash;
        PasswordSalt = salt;
        PasswordEncrypted = EncryptionService.Encrypt(password, EncryptionService.MasterKey);
    }

    // Check a plaintext password for login
    public bool ValidatePassword(string password) =>
        EncryptionService.VerifyHash(password, PasswordHash, PasswordSalt);

    public string GetPasswordForDisplay(string adminKey)
        => EncryptionService.Decrypt(PasswordEncrypted, adminKey);
}