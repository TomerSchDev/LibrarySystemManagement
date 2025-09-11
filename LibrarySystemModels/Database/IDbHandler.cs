using LibrarySystemModels.Models;

namespace LibrarySystemModels.Database;

public interface IDbHandler
{
    void InitializeDatabase(string? newPath);
    void Insert<T>(T obj)where T : new();
    void Update<T>(T obj)where T : new();
    void Delete<T>(int id) where T : new();
    List<T> SelectAll<T>() where T : new();
    User? GetCurrentUser();
    User? Login(string username, string password);

    string Name { get; }
}