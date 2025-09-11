namespace LibrarySystemModels.Models;

public record Book() : IExportable
{
    public Book(string title, string author, string isbn,int quantity):this()
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        Quantity = quantity;
        
    }

    public virtual bool Equals(Book? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if  (other.GetType() != this.GetType()) return false;
        return this.Title == other.Title && this.Author == other.Author && this.ISBN == other.ISBN;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Author, ISBN);
    }

    public int BookID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
    public string ExportClassName =>"Book";
}