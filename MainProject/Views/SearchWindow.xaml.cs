using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;

namespace Library_System_Management.Views
{
    public partial class SearchWindow : Window
    {
        private List<Book> _allBooks = new();
        private List<Member> _allMembers = new();

        public SearchWindow()
        {
            InitializeComponent();
            Loaded += SearchWindow_Loaded;

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

        private async void SearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async Task LoadDataAsync()
        {
            var booksRes = await BookService.GetAllBooksAsync(FlowSide.Client);
            _allBooks = booksRes.ActionResult ? booksRes.Data : [];
            dgBooks.ItemsSource = _allBooks;

            var membersRes = await MemberService.GetAllMembersAsync(FlowSide.Client);
            _allMembers = membersRes.ActionResult ? membersRes.Data : [];
            dgMembers.ItemsSource = _allMembers;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = TxtSearch.Text.Trim().ToLower();

            dgBooks.ItemsSource = string.IsNullOrEmpty(query)
                ? _allBooks
                : _allBooks.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(query, System.StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.Author) && b.Author.Contains(query, System.StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(b.ISBN) && b.ISBN.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                ).ToList();

            dgMembers.ItemsSource = string.IsNullOrEmpty(query)
                ? _allMembers
                : _allMembers.Where(m =>
                    (!string.IsNullOrEmpty(m.FullName) && m.FullName.Contains(query, System.StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.Email) && m.Email.Contains(query, System.StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.Phone) && m.Phone.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }
    }
}