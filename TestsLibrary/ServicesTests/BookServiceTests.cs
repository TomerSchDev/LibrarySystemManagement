using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Xunit;

namespace TestsLibrary
{
    public class BookServiceTests : IDisposable
    {
        private readonly string _dbTestPath;

        public BookServiceTests()
        {
            // Unique temp DB for isolation
            _dbTestPath = Path.Combine("Resources", $"test_{Guid.NewGuid()}.sqlite");
            DataBaseService.SetModes(false,true);
            DataBaseService.Init(_dbTestPath);
            
            Utils.LogInUser(null,FlowSide.Client);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (File.Exists(_dbTestPath)) return;
                //File.Delete(_dbTestPath);
        }

        private async Task<bool> AddBookAsync(Book book)
        {
            var res = await BookService.AddBookAsync(FlowSide.Client, book);
            return res.ActionResult;
        }

        private async Task<Book> AddBookToDatabaseAsync()
        {
            var book = new Book
            {
                Title = "Test Book",
                Author = "Author Name",
                ISBN = "1234567890"
            };
            Assert.NotNull(book);
            var result = await AddBookAsync(book);
            Assert.True(result);
            var all = await BookService.GetAllBooksAsync(FlowSide.Client);
            var added = all.Data?.FirstOrDefault(b => b.Title == book.Title && b.Author == book.Author);
            Assert.NotNull(added);
            return added!;
        }

        private async Task<int> GetBookIdAsync(Book book)
        {
            var books = await BookService.GetAllBooksAsync(FlowSide.Client);
            var found = books.Data?.FirstOrDefault(b => b.Title == book.Title && b.Author == book.Author && b.ISBN == book.ISBN);
            Assert.NotNull(found);
            return found.BookID;
        }

        [Fact]
        public async Task CanAddBook()
        {
            var book = new Book
            {
                Title = "Test Book",
                Author = "Author Name",
                ISBN = "1234567890"
            };
            Assert.NotNull(book);
            var result = await AddBookAsync(book);
            Assert.True(result);
        }

        [Fact]
        public async Task CanDeleteBook()
        {
            var book = await AddBookToDatabaseAsync();
            var bookId = await GetBookIdAsync(book);

            var deleteResult = await BookService.DeleteBookAsync(FlowSide.Server, bookId);
            Assert.True(deleteResult.ActionResult);

            var afterDelete = await BookService.GetBookByIdAsync(FlowSide.Server, bookId);
            Assert.False(afterDelete.ActionResult);
        }

        [Fact]
        public async Task CanUpdateBook()
        {
            var book = await AddBookToDatabaseAsync();
            var bookId = await GetBookIdAsync(book);

            book.BookID = bookId;
            book.Title = "Test Book 2";
            book.Author = "Author Name 2";

            var updateResult = await BookService.UpdateBookAsync(FlowSide.Server, book);
            Assert.True(updateResult.ActionResult);

            var updated = await BookService.GetBookByIdAsync(FlowSide.Server, bookId);
            Assert.Equal("Test Book 2", updated.Data?.Title);
            Assert.Equal("Author Name 2", updated.Data?.Author);
        }
    }
}
