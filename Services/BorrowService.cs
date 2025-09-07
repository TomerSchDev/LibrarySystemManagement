using System.Reflection;
using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;

namespace Library_System_Management.Services
{
    public static class BorrowService
    {
        public static void IssueBook(int bookId, int memberId, DateTime? returnDate)
        {
            var borrowedBook = new BorrowedBook
            {
                BookId = bookId,
                MemberId = memberId,
                IssueDate = DateTime.Now,
                ReturnDate = returnDate ?? DateTime.Now.AddDays(14),
                Returned = false
            };
            try
            {
                DatabaseManager.Insert(borrowedBook);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't insert borrowed book : "+e.Message , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ReturnBook(int borrowId)
        {
            List<BorrowedBook> borows;
            try
            {
                borows = DatabaseManager.SelectAll<BorrowedBook>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sellecting all BorrowedBook : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if ((borows.Count == 0))
            {
                MessageBox.Show("Couldent found any boorow books.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var borrow = borows.FirstOrDefault(b => b != null && b.BorrowId == borrowId);
            if (borrow == null)
            {
                MessageBox.Show($"Error finding Borrow book with id: {borrowId}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (borrow.Returned)
            {
                MessageBox.Show("This book was already returned.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            List<Book> books ;
            try
            {
                books = DatabaseManager.SelectAll<Book>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sellecting all books : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var book = books.FirstOrDefault(b => b != null && b.BookID == borrow.BookId);
            if (book == null)
            {
                MessageBox.Show($"Error finding Book with id: {borrow.BookId}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            if (book.Available >= book.Quantity)
            {
                MessageBox.Show($"Book '{book.Title}' already has all copies available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            borrow.Returned = true;
            borrow.ReturnDate = DateTime.Now;
            try
            {
                DatabaseManager.Update(borrow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating borrow book id: {borrow.BorrowId} : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            book.Available++;
            try
            {
                DatabaseManager.Update(book);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating  book id: {borrow.BookId} : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                borrow.Returned = false;
                DatabaseManager.Update(borrow);
                return;
            }
        }


        private static List<BorrowedBook> GetAllBorrowedBooks()
        {
            var list = new List<BorrowedBook>();
            try
            {
                list = DatabaseManager.SelectAll<BorrowedBook>();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error selecting all BorrowedBook data from db with error:"+ e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return list;
        }

        private static List<BorrowedBookView> GetBorrowedBooksHistoryByCondition(Func<BorrowedBookView,bool> condition)
        {
            var allBorrowedBooks = GetAllBorrowedBooks();
            var memberBorrowBooks=new  List<BorrowedBookView>();
            memberBorrowBooks.AddRange(allBorrowedBooks.Select(b => new BorrowedBookView
            {
                BorrowID = b.BorrowId,
                BookID = b.BookId,
                MemberID = b.MemberId,
                BorrowDate = b.IssueDate,
                ExpectedReturnDate = b.ReturnDate,
                ReturnDate = b.ReturnDate,
                Returned = b.Returned,
                Book = BookService.GetBookById(b.BookId),
            }).Where(condition).ToList());
            return memberBorrowBooks;
        }

        public static List<BorrowedBookView> GetBorrowHistoryByMemberId(int memberId)
        {
            return GetBorrowedBooksHistoryByCondition(b=>b.MemberID == memberId);
        }
        public static List<BorrowedBookView> GetBorrowHistoryByBookId(int bookId)
        {
            return GetBorrowedBooksHistoryByCondition(b=>b.BookID==bookId);
        }
    }
}
