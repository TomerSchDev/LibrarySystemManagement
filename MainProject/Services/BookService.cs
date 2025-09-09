using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Microsoft.Data.Sqlite;

namespace Library_System_Management.Services
{
    public static class BookService
    {
        public static bool AddBook(Book book)
        {
            if (!SessionHelperService.IsEnoughPermission(UserRole.Librarian))
            {
                MessageBox.Show("You do not have permission to add books! , action was written to report","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,"User try to add new book without enough permission");
                return false;
            };
            try
            {
                DatabaseManager.Insert(book);
            }
            catch (Exception e)
            {
                var bookInfo =book.Title!=string.Empty?book.Title:"bookId : "+book.BookID;
                MessageBox.Show($"couldn't insert new book {bookInfo}: "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,"User try to insert new book and couldn't be inserted because of DB error");
                return false;
            }

            return true;
        }

        public static bool UpdateBook(Book book)
        {
            var bookInfo =book.Title!=string.Empty?"book : "+book.Title:"bookId : "+book.BookID;
            if (!SessionHelperService.IsEnoughPermission(UserRole.Librarian))
            {
                MessageBox.Show("You do not have permission to update book! , action was written to report","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,$"User try to update {bookInfo}  without enough permission");
                return false;
            }

            try
            {
                DatabaseManager.Update(book);
            }
            catch (Exception e)
            {
                MessageBox.Show($"couldn't update book {bookInfo} "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,$"User try to update {bookInfo} and couldn't be inserted because of DB error");
                return false;

            }
            return true;
        }

        public static bool DeleteBook(int id)
        {
            if (!SessionHelperService.IsEnoughPermission(UserRole.Admin))
            {
                MessageBox.Show("You do not have permission to delete book! , action was written to report","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,$"User try to delete book id: {id}  without enough permission");
                return false;

            }

            try
            {
                DatabaseManager.Delete<Book>(id);
            }
            catch (Exception e)
            {
                MessageBox.Show($"couldn't delete book {id} "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ReportingService.ReportEvent(SeverityLevel.LOW,$"User try to delete bookID {id} and couldn't be inserted because of DB error");
                return false;
            }

            return true;
        }
        public static List<Book> GetAllBooks() => DatabaseManager.SelectAll<Book>();

        public static Book? GetBookById(int id)
        {
            var allBooks = GetAllBooks();
            return allBooks.FirstOrDefault(book => book.BookID == id);
        }
    }
}
