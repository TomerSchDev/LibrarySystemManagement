using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views.PopUpDialogs
{
    public partial class AddBorrowWindow : Window
    {
        private readonly Member _member;
        public BorrowedBookView? NewBorrow { get; private set; }
        public AddBorrowWindow(Member member)
        {
            InitializeComponent();
            _member = member;

            // Load available books
            var allBooks = BookService.GetAllBooks();
            cmbBooks.ItemsSource =allBooks.Where(book => book.Available >= 0).ToList();
            dpBorrowDate.SelectedDate = DateTime.Now;
            dpReturnDate.SelectedDate = DateTime.Now.AddDays(14);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBooks.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book.");
                return;
            }
            NewBorrow = new BorrowedBookView
            {
                BookID = selectedBook.BookID,
                MemberID = _member.MemberID,
                ReturnDate = dpReturnDate.SelectedDate
            };
            DialogResult = true;
            Close();
        }
    }
}