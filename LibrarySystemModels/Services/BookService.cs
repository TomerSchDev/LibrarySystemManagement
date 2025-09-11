using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
// Add this!

// For MessageBox

namespace LibrarySystemModels.Services
{
    public static class BookService
    {
        private const string BookServiceUrl = "api/Books/";

        public static async Task<ResultResolver<Book>> AddBookAsync(FlowSide side, Book book)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to add books! , action was written to report";
                if (side != FlowSide.Client) return new ResultResolver<Book>(null!, false, errorMessage);
                
                await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Book>(null!, false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Insert<Book,Book>(BookServiceUrl, book);

            try
            {
                // Local DB is fast, but if you want to keep everything async:
                await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(book));
                return new ResultResolver<Book>(book, true, "");
            }
            catch (Exception e)
            {
                var bookInfo = !string.IsNullOrEmpty(book.Title) ? book.Title : "bookId : " + book.BookID;
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, $"User tried to insert new book : {bookInfo} and couldn't because of DB error : {e.Message}");
                return new ResultResolver<Book>(null!, false, $"couldn't insert new book : {bookInfo}");
            }
        }

        public static async Task<ResultResolver<Book>> UpdateBookAsync(FlowSide side, Book book)
        {
            var bookInfo = !string.IsNullOrEmpty(book.Title) ? "book : " + book.Title : "bookId : " + book.BookID;
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                var errorMessage = $"User try to update {bookInfo} without enough permission";
                if (side == FlowSide.Client)
                {
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Update<Book,Book>(BookServiceUrl, book);

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Update(book));
                return new ResultResolver<Book>(book, true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed updating book {bookInfo} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<Book>> DeleteBookAsync(FlowSide side, int id)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Admin))
            {
                var errorMessage = $"User try to delete book id: {id} without enough permission";
                if (side == FlowSide.Client)
                {
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Delete<Book>(BookServiceUrl + $"{id}");

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Delete<Book>(id));
                return new ResultResolver<Book>(new Book(), true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to delete book {id} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<List<Book>>> GetAllBooksAsync(FlowSide side)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to get Books! , action was written to report";
                if (side != FlowSide.Client)
                    return new ResultResolver<List<Book>>(new List<Book>(), false, errorMessage);
                await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<List<Book>>(new List<Book>(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<Book>>(BookServiceUrl);

            try
            {
                var books = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Book>());
                
                return new ResultResolver<List<Book>>(books, true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"couldn't get all books because of DB error: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<List<Book>>(new List<Book>(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<Book>> GetBookByIdAsync(FlowSide side, int id)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to get Book! , action was written to report";
                if (side == FlowSide.Client)
                {
                    await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Get<Book>(BookServiceUrl + $"{id}");

            try
            {
                var books = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Book>());
                var book = books.FirstOrDefault(b => b.BookID == id);
                if (book != null)
                    return new ResultResolver<Book>(book, true, "");
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, "Book not found :" + id);
                return new ResultResolver<Book>(new Book(), false, $"Error finding book id: {id}");
            }
            catch (Exception e)
            {
                var errorMessage = $"couldn't get book {id} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Book>(new Book(), false, errorMessage);
            }
        }
    }
}
