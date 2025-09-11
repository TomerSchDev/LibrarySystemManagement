using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;
using System.Threading.Tasks;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views.PopUpDialogs
{
    public partial class AddIssueBook : Window
    {
        public BorrowedBookView? NewBorrow { get; private set; }

        public AddIssueBook()
        {
            InitializeComponent();
            Loaded += AddIssueBook_Loaded;
        }

        private async void AddIssueBook_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadContentAsync();
            }
            catch (Exception ez)
            {
                MessageBoxService.ShowMessage(ez);
            }
        }

        private async Task LoadContentAsync()
        {
            cmbBooks.ItemsSource = null;
            cmbMembers.ItemsSource = null;
            
            var membersResult = await MemberService.GetAllMembersAsync(FlowSide.Client);
            if (membersResult.ActionResult)
                cmbMembers.ItemsSource = membersResult.Data;
            else
                MessageBox.Show("Could not load members.\n" + membersResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            var booksResult = await BookService.GetAllBooksAsync(FlowSide.Client);
            if (booksResult is { ActionResult: true, Data: not null })
                cmbBooks.ItemsSource = booksResult.Data.Where(b => b.Available > 0).ToList();
            else
                MessageBox.Show("Could not load books.\n" + booksResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBooks.SelectedItem is not Book selectedBook)
            {
                MessageBox.Show("Please select a book.");
                return;
            }

            if (cmbMembers.SelectedItem is not Member selectedMember)
            {
                MessageBox.Show("Please select a member first.");
                return;
            }

            if (dpReturnDate.SelectedDate is null)
            {
                MessageBox.Show("Please select an expected return date.");
                return;
            }

            NewBorrow = new BorrowedBookView
            {
                BookID = selectedBook.BookID,
                MemberID = selectedMember.MemberID,
                Book = selectedBook,
                Member = selectedMember,
                BorrowDate = DateTime.Now,
                ExpectedReturnDate = dpReturnDate.SelectedDate.Value
            };

            DialogResult = true;
            Close();
        }
    }
}
