using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Database;

public class RestApiConnector(string baseApiUrl)
{
    private readonly HttpClient _httpClient = new();
    private AuthenticationHeaderValue _token;
    private static async Task<ResultResolver<TResult>> ResolveRequestAsync<TResult>(HttpResponseMessage responseMessage)
    {
        if (!responseMessage.IsSuccessStatusCode)
            return new ResultResolver<TResult>(default!, false, $"Got error {responseMessage.StatusCode}: {responseMessage.ReasonPhrase}");
        var result =  responseMessage.Content.ReadFromJsonAsync<ResultResolver<TResult>>().Result;
        return result ?? new ResultResolver<TResult>(default!, false, "No result in response");
    }

    public async Task<ResultResolver<TResult>> PostRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res =  _httpClient.PostAsJsonAsync(baseApiUrl+url, payload);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public async Task<ResultResolver<TResult>> PutRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res =  _httpClient.PutAsJsonAsync(baseApiUrl+url, payload);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public async Task<ResultResolver<TResult>> DeleteRequestAsync<TResult>(string url)
    {
        var res =  _httpClient.DeleteAsync(baseApiUrl+url);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public async Task<ResultResolver<TResult>> GetRequestAsync<TResult>(string url)
    {
        var urlFull = baseApiUrl + url;
        Console.WriteLine(urlFull);
       
        var res =  _httpClient.GetAsync(urlFull).Result;
        var str =res.Content.ReadAsStringAsync();
        Console.WriteLine("RAW JSON: " + str);
        return await ResolveRequestAsync<TResult>(res);
    }

    // Example for Login
    public async Task<ResultResolver<User>> LoginAsync(string username, string password)
    {
        var res =  await _httpClient.PostAsJsonAsync($"{baseApiUrl}api/Auth/login", new { username, password });
        if (!res.IsSuccessStatusCode)return new ResultResolver<User>(User.DefaultUser, false, $"Got error from server {res.StatusCode}");
        var loginResponse = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (User.IsDefaultUser(loginResponse!.User)) return new ResultResolver<User>(User.DefaultUser, false, $"Got error from server {res.StatusCode}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.Token);
        _token= new AuthenticationHeaderValue("Bearer", loginResponse.Token);
        return new ResultResolver<User>(loginResponse.User, true, $"Got ");
    }
}
