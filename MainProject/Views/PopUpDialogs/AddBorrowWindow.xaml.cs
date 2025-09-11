using System.Windows;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;
using LibrarySystemModels.Services;

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
            Loaded += AddBorrowWindow_Loaded;
        }

        private async void AddBorrowWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Asynchronously load books
                var allBooksResult = await BookService.GetAllBooksAsync(FlowSide.Client);
                if (allBooksResult.ActionResult)
                {
                    cmbBooks.ItemsSource = allBooksResult.Data.Where(book => book.Available > 0).ToList();
                }
                else
                {
                    MessageBox.Show("Could not load books.\n" + allBooksResult.Message, "Book Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                dpBorrowDate.SelectedDate = DateTime.Now;
                dpReturnDate.SelectedDate = DateTime.Now.AddDays(14);
            }
            catch (Exception exception)
            {
                MessageBoxService.ShowMessage(exception);
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBooks.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book.");
                return;
            }

            if (dpReturnDate.SelectedDate is null)
            {
                MessageBox.Show("Please select an expected return date.");
                return;
            }

            // Optionally validate returns after borrow date, etc.

            NewBorrow = new BorrowedBookView
            {
                BookID = selectedBook.BookID,
                MemberID = _member.MemberID,
                BorrowDate = dpBorrowDate.SelectedDate ?? DateTime.Now,
                ExpectedReturnDate = dpReturnDate.SelectedDate.Value
            };

            DialogResult = true;
            Close();
        }
    }
}
