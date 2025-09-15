using System.Net.Http.Headers;
using System.Net.Http.Json;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;

namespace LibrarySystemModels.Database.Servers;

public class RestApiServer:DataServers
{
    private static RestApiServer? _server = null;
    private AuthenticationHeaderValue? _token;
    private string? _baseUrl;

    public static DataServers GetServer()
    {
        _server ??= new RestApiServer();
        return _server;
    }

    private readonly HttpClient _client;

    private RestApiServer()
    {
        _client = new HttpClient();
    }
    public override async Task<bool> Connect(string address)
    {
        var url = address;
        if (!address.StartsWith("http://"))  url= "http://" + address;
        if (!address.EndsWith("/")) url += "/";
        var grethel = url + "health";
        Console.WriteLine(grethel);
        var response = await _client.GetAsync(grethel);
        if  (!response.IsSuccessStatusCode) return false;
        var payload = await response.Content.ReadFromJsonAsync<ConnectionHealth>();
        
        return payload is { status: "ok" };
    }

    public override string ServerTypeName() => "Rest Api Server";
     private static async Task<ResultResolver<TResult>> ResolveRequestAsync<TResult>(HttpResponseMessage responseMessage)
    {
        if (!responseMessage.IsSuccessStatusCode)
            return new ResultResolver<TResult>(default!, false, $"Got error {responseMessage.StatusCode}: {responseMessage.ReasonPhrase}");
        var result =  responseMessage.Content.ReadFromJsonAsync<ResultResolver<TResult>>().Result;
        return result ?? new ResultResolver<TResult>(default!, false, "No result in response");
    }

    public override async Task<ResultResolver<TResult>> PostRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res =  _client.PostAsJsonAsync(_baseUrl+url, payload);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public override async Task<ResultResolver<TResult>> PutRequestAsync<TResult, TPayload>(string url, TPayload payload)
    {
        var res =  _client.PutAsJsonAsync(_baseUrl+url, payload);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public override async Task<ResultResolver<TResult>> DeleteRequestAsync<TResult>(string url)
    {
        var res =  _client.DeleteAsync(_baseUrl+url);
        return await ResolveRequestAsync<TResult>(res.Result);
    }

    public override async Task<ResultResolver<TResult>> GetRequestAsync<TResult>(string url)
    {
        var urlFull = _baseUrl + url;
        Console.WriteLine(urlFull);
       
        var res =  _client.GetAsync(urlFull).Result;
        var str =res.Content.ReadAsStringAsync();
        Console.WriteLine("RAW JSON: " + str);
        return await ResolveRequestAsync<TResult>(res);
    }

    // Example for Login
    public override async Task<ResultResolver<User>> LoginAsync(string username, string password)
    {
        var res =  await _client.PostAsJsonAsync($"{_baseUrl}api/Auth/login", new { username, password });
        if (!res.IsSuccessStatusCode)return new ResultResolver<User>(User.DefaultUser, false, $"Got error from server {res.StatusCode}");
        var loginResponse = await res.Content.ReadFromJsonAsync<LoginResponse>();
        if (User.IsDefaultUser(loginResponse!.User)) return new ResultResolver<User>(User.DefaultUser, false, $"Got error from server {res.StatusCode}");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.Token);
        _token= new AuthenticationHeaderValue("Bearer", loginResponse.Token);
        return new ResultResolver<User>(loginResponse.User, true, $"Got ");
    }
}