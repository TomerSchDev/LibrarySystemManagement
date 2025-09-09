using System.Security.Cryptography;
using System.Text;
using Library_System_Management.Helpers;

namespace Library_System_Management.Services;

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
    // Hash & salt
    public static (string hash, string salt) CreateHash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        var hash = Convert.ToBase64String(hashBytes);
        return (hash, salt);
    }

    public static bool VerifyHash(string password, string storedHash, string storedSalt)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + storedSalt));
        var hash = Convert.ToBase64String(hashBytes);
        return hash == storedHash;
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

    public static string Decrypt(string cipherText, string key)
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
