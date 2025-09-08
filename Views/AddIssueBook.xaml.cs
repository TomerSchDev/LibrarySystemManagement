using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class AddIssueBook : Window
{
    public BorrowedBookView? NewBorrow { get; private set; }

    public AddIssueBook()
    {
        InitializeComponent();
        LoadContent();
    }

    private void LoadContent()
    {
        cmbBooks.ItemsSource = null;
        cmbMembers.ItemsSource = null;
        cmbMembers.ItemsSource = MemberService.GetAllMembers();
        cmbBooks.ItemsSource=BookService.GetAllBooks().Where(b=>b.Available>0).ToList();
    }
    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (cmbBooks.SelectedItem is not Book selectedBook)
        {
            MessageBox.Show("Please select a book.");
            return;
        }

        if (cmbMembers.SelectionBoxItem is not Member selectedMember)
        {
            MessageBox.Show("Please select a member first.");
            return;
        }
        NewBorrow = new BorrowedBookView
        {
            BookID = selectedBook.BookID,
            MemberID = selectedMember.MemberID,
            Book = selectedBook,
            Member = selectedMember,
            ReturnDate = dpReturnDate.SelectedDate
        };
        DialogResult = true;
        Close();
    }
}