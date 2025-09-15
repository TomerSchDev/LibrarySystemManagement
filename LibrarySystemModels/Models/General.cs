using LibrarySystemModels.Services;

namespace LibrarySystemModels.Models;

public static class General
{
    public static readonly string Password = "Library Connection Key";
    public static readonly string SystemPrivateKey = EncryptionService.Encrypt(Password,EncryptionService.MasterKey);

}