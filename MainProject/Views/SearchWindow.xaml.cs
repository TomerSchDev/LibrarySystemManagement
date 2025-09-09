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
            if (dgBooks.SelectedItem is not Book book) return;
            var win = new BookInfoWindow(book);
            win.ShowDialog();
        };

        dgMembers.MouseDoubleClick += (s, e) =>
        {
            if (dgMembers.SelectedItem is not Member member) return;
            var win = new MemberInfoWindow(member);
            win.ShowDialog();
        };
    }
    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = TxtSearch.Text.Trim();

        // Books search → Title, Author, ISBN
        var books = BookService.GetAllBooks()
            .Where(b =>
                (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(b.Author) && b.Author.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(b.ISBN) && b.ISBN.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase))
            )
            .ToList();
        dgBooks.ItemsSource = books;

        // Members search → FullName, Email, Phone
        var members = MemberService.GetAllMembers()
            .Where(m =>
                (!string.IsNullOrEmpty(m.FullName) && m.FullName.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(m.Email) && m.Email.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(m.Phone) && m.Phone.ToLower().Contains(query,StringComparison.CurrentCultureIgnoreCase))
            )
            .ToList();
        dgMembers.ItemsSource = members;
    }

}