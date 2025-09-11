namespace LibrarySystemModels.Models;

public record ResultResolver<T>(T Data, bool ActionResult, string Message)
{
   

    public T Data {get; set;} = Data;
    public bool ActionResult {get; set;} = ActionResult;
    public string Message {get; set;} = Message;
}