using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Database;

public class RestApiConnector(string baseApiUrl)
{
    private readonly HttpClient _httpClient = new();

    private static async Task<ResultResolver<TResult>> ResolveRequestAsync<TResult>(HttpResponseMessage responseMessage)
    {
        if (!responseMessage.IsSuccessStatusCode)
            return new ResultResolver<TResult>(default!, false, $"Got error {responseMessage.StatusCode}: {responseMessage.ReasonPhrase}");
        var result = await responseMessage.Content.ReadFromJsonAsync<TResult>();
        return result == null
            ? new ResultResolver<TResult>(default!, false, "No result in response")
            : new ResultResolver<TResult>(result, true, "");
    }

    public async Task<ResultResolver<TResult>> PostRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res = await _httpClient.PostAsJsonAsync(url, payload);
        return await ResolveRequestAsync<TResult>(res);
    }

    public async Task<ResultResolver<TResult>> PutRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res = await _httpClient.PutAsJsonAsync(url, payload);
        return await ResolveRequestAsync<TResult>(res);
    }

    public async Task<ResultResolver<TResult>> DeleteRequestAsync<TResult>(string url)
    {
        var res = await _httpClient.DeleteAsync(url);
        return await ResolveRequestAsync<TResult>(res);
    }

    public async Task<ResultResolver<TResult>> GetRequestAsync<TResult>(string url)
    {
        var res = await _httpClient.GetAsync(url);
        return await ResolveRequestAsync<TResult>(res);
    }

    // Example for Login
    public async Task<ResultResolver<User>> LoginAsync(string username, string password)
    {
        var res = await _httpClient.PostAsJsonAsync($"{baseApiUrl}/api/Auth/login", new { username, password });
        return await ResolveRequestAsync<User>(res);
    }
}
