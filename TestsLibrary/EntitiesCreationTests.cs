using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Xunit;

namespace TestsLibrary;

public class EntitiesCreationTests
{
    [Fact]
    public void CanCreateUser()
    {
        var user = AuthService.CreateUser("testuser", "testpass", UserRole.Librarian);
        Assert.Equal("testuser", user.Username);
        Assert.True(EncryptionService.VerifyHash(user,"testpass"));
        Assert.Equal(UserRole.Librarian, user.Role);
    }

    [Fact]
    public void CanCreateMember()
    {
        var member = new Member("John Doe", "john@example.com", "12345678");
        Assert.Equal("John Doe", member.FullName);
        Assert.Equal("john@example.com", member.Email);
        Assert.Equal("12345678", member.Phone);
    }

    [Fact]
    public void CanCreateBook()
    {
        var book = new Book
        {
            Title = "Test Book",
            Author = "Author Name",
            ISBN = "1234567890"
        };
        Assert.Equal("Test Book", book.Title);
        Assert.Equal("Author Name", book.Author);
        Assert.Equal("1234567890", book.ISBN);
    }
}