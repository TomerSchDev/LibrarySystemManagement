using System.Diagnostics;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Library_System_Management.Services;
using Xunit;

namespace TestsLibrary;

public class BookServiceTests :IDisposable
{
    private readonly string _dbTestPath;

    public BookServiceTests()
    {
        // Use a unique temp database file for each test class instance
        _dbTestPath = Path.Combine("Resources", $"test_{Guid.NewGuid()}.sqlite");
        DatabaseManager.InitializeDatabaseForTest(_dbTestPath);
    }

    public void Dispose()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        // Cleanup: remove temp database after each test class instance
        if (File.Exists(_dbTestPath)) return;
        File.Delete(_dbTestPath);
    }
    
    
    [Fact]
    public void CanAddBook()
    {
        var book = new Book
        {
            Title = "Test Book",
            Author = "Author Name",
            ISBN = "1234567890"
        };
        Assert.NotNull(book);
        Assert.True(BookService.AddBook(book));
    }

    private static Book AddBookToDatabase()
    {
        var book = new Book
        {
            Title = "Test Book",
            Author = "Author Name",
            ISBN = "1234567890"
        };
        Assert.NotNull(book);
        Assert.True(BookService.AddBook(book));
        return book;
    }

    private static int GetBookId(Book book)
    {
        var books= BookService.GetAllBooks();
        return books.FirstOrDefault(b => b.Equals(book))!.BookID;
    }

    [Fact]
    public void CanDeleteBook()
    {
        AddBookToDatabase();
        var bookId = GetBookId(AddBookToDatabase());
        Assert.True(BookService.DeleteBook(bookId));
    }
    [Fact]
    public void CanUpdateBook()
    {
        var b = AddBookToDatabase();
        var bookId= GetBookId(b);
        b.BookID = bookId;
        b.Title = "Test Book2"; //change it so it change
        b.Author = "Author Name2";
        Assert.False(b.Equals(BookService.GetBookById(bookId)));
        Assert.True(BookService.UpdateBook(b));
    }
}