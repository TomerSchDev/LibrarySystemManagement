using LibrarySystemModels.Models;
using LibrarySystemModels.Services;

namespace LibrarySystemModels;

public abstract partial class DataServers
{
    public abstract Task<bool> Connect(string address);

    public abstract string ServerTypeName();
    public abstract Task<ResultResolver<TResult>> PostRequestAsync<TResult, TPayload>(string url, TPayload payload) ; 
    public abstract Task<ResultResolver<TResult>> PutRequestAsync<TResult, TPayload>(string url, TPayload payload);
    public abstract Task<ResultResolver<TResult>> DeleteRequestAsync<TResult>(string url);
    public abstract Task<ResultResolver<TResult>> GetRequestAsync<TResult>(string url);
    public abstract Task<ResultResolver<User>> LoginAsync(string username, string password);


}

public record ConnectionHealth()
{
    public string status { get; init; }
}