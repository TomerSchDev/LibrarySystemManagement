namespace Library_System_Management.Models;

public record Book() : IExportable
{
    public Book(string title, string author, string isbn,int quantity):this()
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        Quantity = quantity;
        
    }

    public int BookID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
    public string ExportClassName =>"Book";
}