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
            try
            {
                DatabaseManager.Insert(book);
            }
            catch (Exception e)
            {
                var bookInfo =book.Title!=string.Empty?book.Title:"bookId : "+book.BookID;
                MessageBox.Show($"couldn't insert new book {bookInfo}: "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool UpdateBook(Book book)
        {
            try
            {
                DatabaseManager.Update(book);
            }
            catch (Exception e)
            {
                var bookInfo =book.Title!=string.Empty?book.Title:"bookId : "+book.BookID;
                MessageBox.Show($"couldn't update book {bookInfo} "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;

            }
            return true;
        }

        public static bool DeleteBook(int id)
        {
            try
            {
                DatabaseManager.Delete<Book>(id);
            }
            catch (Exception e)
            {
                MessageBox.Show($"couldn't delete book {id} "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
