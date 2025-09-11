using LibrarySystemModels.Database;
using LibrarySystemModels.Models;
using System.Threading.Tasks;
using LibrarySystemModels.Helpers;

namespace LibrarySystemModels.Services;

public static class DataBaseService
{
    private static RestApiConnector? RestApiConnector;
    private static LocalDatabaseHandler LocalDatabaseHandler;

    private static bool IsLocalMode { get; set; } 
    private static bool _isServerMode ;
    private static bool Initilized ;


    
    public static void SetModes(bool isServerMode, bool isLocalMode)
    {
        if (Initilized)return;
        _isServerMode=isServerMode;
        IsLocalMode=isLocalMode;
        Initilized = true;
    }

public static void Init(string? path)
    {
        LocalDatabaseHandler = new LocalDatabaseHandler();
        LocalDatabaseHandler.InitializeDatabase(path);
        if (Initilized && (_isServerMode ||IsLocalMode))
        {
            return;
        }
        SetModes(false, false);
        
        if (_isServerMode) return;
        try
        {
            var url = "http://localhost:5164";
            var fileUrlPath = FileRetriever.RetrieveFIlePath("ApiBaseUrl.txt");
            if (File.Exists(fileUrlPath))
            {
                url = File.ReadAllText(fileUrlPath);
            }

            if (string.IsNullOrEmpty(url))
            {
                IsLocalMode = true;
                RestApiConnector = null;
            }

            Console.WriteLine($"app path : {url}");
            IsLocalMode = false;
            RestApiConnector = new RestApiConnector(url + "/");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            IsLocalMode = true;
            RestApiConnector = null;
        }
    }
    public static async Task<ResultResolver<TResult>> Insert<TResult, TPayload>(string url, TPayload payload)
    {
        if (IsLocalMode)
            return await LocalApiSimulator.InsertAsync<TResult, TPayload>(url, payload);
        return await RestApiConnector.PostRequestAsync<TResult, TPayload>(url, payload);
    }

    public static async Task<ResultResolver<TResult>> Update<TResult, TPayload>(string url, TPayload payload)
    {
        if (IsLocalMode)
            return await LocalApiSimulator.UpdateAsync<TResult, TPayload>(url, payload);
        else
            return await RestApiConnector.PutRequestAsync<TResult, TPayload>(url, payload);
    }

    public static async Task<ResultResolver<TResult>> Delete<TResult>(string url)
    {
        if (IsLocalMode)
            return await LocalApiSimulator.DeleteAsync<TResult>(url);
        else
            return await RestApiConnector.DeleteRequestAsync<TResult>(url);
    }

    public static async Task<ResultResolver<TResult>> Get<TResult>(string url)
    {
        if (IsLocalMode)
            return await LocalApiSimulator.GetAsync<TResult>(url);
        else
            return await RestApiConnector.GetRequestAsync<TResult>(url);
    }

    public static LocalDatabaseHandler GetLocalDatabase() => LocalDatabaseHandler;

    public static async Task<ResultResolver<User>> Login(string username, string password)
    {
        
        if (IsLocalMode)
            return await LocalApiSimulator.LoginAsync(username, password);
        else
            return await RestApiConnector.LoginAsync(username, password);
    }
}
