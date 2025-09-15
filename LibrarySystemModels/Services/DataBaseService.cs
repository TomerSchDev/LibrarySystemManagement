using LibrarySystemModels.Database;
using LibrarySystemModels.Models;
using System.Threading.Tasks;
using LibrarySystemModels.Helpers;

namespace LibrarySystemModels.Services;

public static class DataBaseService
{
    private static DataServers? _dataServer;
    private static readonly AsyncLocal<LocalDatabaseHandler?> LocalDb = new();
    
    public static void SetDataServer(DataServers dataServer)
    {
        if (LocalDb.Value == null) InitLocalDb(null);
        _dataServer=dataServer;
    }
    public static async Task<ResultResolver<TResult>> Insert<TResult, TPayload>(string url, TPayload payload)
    {
        return await _dataServer?.PostRequestAsync<TResult, TPayload>(url, payload)!;
    }

    public static async Task<ResultResolver<TResult>> Update<TResult, TPayload>(string url, TPayload payload)
    {
            return await _dataServer?.PutRequestAsync<TResult, TPayload>(url, payload)!;
    }

    public static async Task<ResultResolver<TResult>> Delete<TResult>(string url)
    {
        return await _dataServer?.DeleteRequestAsync<TResult>(url)!;
    }

    public static async Task<ResultResolver<TResult>> Get<TResult>(string url)
    {

            return await _dataServer?.GetRequestAsync<TResult>(url)!;
    }
    
    public static LocalDatabaseHandler? GetLocalDatabase()
    {
        if (LocalDb.Value == null) InitLocalDb(null);
        return LocalDb.Value;
    }

    public static void InitLocalDb(string? path)
    {
        LocalDb.Value = new LocalDatabaseHandler();
        LocalDb.Value.InitializeDatabase(path);

    }

    public static async Task<ResultResolver<User>> Login(string username, string password)
    {
        
            return await _dataServer?.LoginAsync(username, password)!;
    }
}
