using System.Security.Cryptography;
using System.Text;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Services
{
public static class EncryptionService
{
    // WARNING: Use strong entropy in production. Demo only!
    public static readonly string MasterKey;

    static EncryptionService()
    {
        try
        {
            var key = ConfigHelper.GetString("Keys:MasterKey");
            MasterKey = key ?? "Test Key";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("using test key"); // todo for real application close the program maybe?
            MasterKey =  "Test Key";
        }
    }

    public static string SetPassword(string password)
    {
        var (hash, salt) = EncryptionService.CreateHash(password);
        var PasswordHash = hash;
        var PasswordSalt = salt;
        return Encrypt(password, MasterKey);
    }
    // Hash & salt
    public static (string hash, string salt) CreateHash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
            var hash = Convert.ToBase64String(hashBytes);
            return (hash, salt);
        }
    }

    public static bool VerifyHash(User user,string password)
    {
        var passwordHash = user.PasswordHash;
        var passwordSalt = user.PasswordSalt;
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + passwordSalt));
        var hash = Convert.ToBase64String(hashBytes);
        return hash == passwordHash;
    }

    // Symmetric encrypt/decrypt for password recovery
    public static string Encrypt(string text, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = new byte[16]; // DEMO: Zero IV
        var encryptor = aes.CreateEncryptor();
        var buffer = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
    }

    public static string DecryptPassword(User user)
    {
        return Decrypt(user.PasswordEncrypted, MasterKey);
    }

    private static string Decrypt(string cipherText, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = new byte[16];
        var decryptor = aes.CreateDecryptor();
        var buffer = Convert.FromBase64String(cipherText);
        return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length));
    }
}
}