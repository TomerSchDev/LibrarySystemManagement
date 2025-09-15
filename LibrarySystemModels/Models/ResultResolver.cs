namespace LibrarySystemModels.Models;

public record ResultResolver<T>()
{
    public ResultResolver(T data, bool actionResult, string message) : this()
    {
        Data = data;
        ActionResult = actionResult;
        Message = message;
    }


    public T Data {get; set;}
    public bool ActionResult {get; set;}
    public string Message {get; set;}
}