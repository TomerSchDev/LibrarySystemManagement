using System.Windows;
using System.Windows.Controls;
using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class SearchWindow : Window
{
    public SearchWindow()
    {
        InitializeComponent();

        // Load all initially
        dgBooks.ItemsSource = BookService.GetAllBooks();
        dgMembers.ItemsSource = MemberService.GetAllMembers();

        // Add double-click events
        dgBooks.MouseDoubleClick += (s, e) =>
        {
            if (dgBooks.SelectedItem is Book book)
            {
                var win = new BookInfoWindow(book);
                win.ShowDialog();
            }
        };

        dgMembers.MouseDoubleClick += (s, e) =>
        {
            if (dgMembers.SelectedItem is Member member)
            {
                var win = new MemberInfoWindow(member);
                win.ShowDialog();
            }
        };
    }
    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = TxtSearch.Text.Trim().ToLower();

        // Books search → Title, Author, ISBN
        var books = BookService.GetAllBooks()
            .Where(b =>
                (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(b.Author) && b.Author.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(b.ISBN) && b.ISBN.ToLower().Contains(query))
            )
            .ToList();
        dgBooks.ItemsSource = books;

        // Members search → FullName, Email, Phone
        var members = MemberService.GetAllMembers()
            .Where(m =>
                (!string.IsNullOrEmpty(m.FullName) && m.FullName.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(m.Email) && m.Email.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(m.Phone) && m.Phone.ToLower().Contains(query))
            )
            .ToList();
        dgMembers.ItemsSource = members;
    }

}