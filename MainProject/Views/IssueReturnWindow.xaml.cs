using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LibrarySystemModels.Services;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models.ViewModels;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views
{
    public partial class IssueReturnWindow : Window
    {
        public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; } = new();
        public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; } = new();

        public IssueReturnWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += IssueReturnWindow_Loaded;
        }

        private async void IssueReturnWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadTablesAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async Task LoadTablesAsync()
        {
            CurrentBorrows.Clear();
            BorrowHistory.Clear();
            try
            {
                var borrowRecordsResult = await BorrowService.GetBorrowHistoryEveryThingAsync(FlowSide.Client);
                if (borrowRecordsResult.ActionResult)
                {
                    foreach (var record in borrowRecordsResult.Data)
                    {
                        if (record.Returned)
                            BorrowHistory.Add(record);
                        else
                            CurrentBorrows.Add(record);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load borrow history: " + borrowRecordsResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrow/return history:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void tbl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var depObj = (DependencyObject)e.OriginalSource;
                while (depObj != null && depObj is not DataGridCell)
                    depObj = VisualTreeHelper.GetParent(depObj);
                if (depObj is not DataGridCell cell) return;

                var header = cell.Column.Header?.ToString() ?? "";
                if (header == "") return;
                if (cell.DataContext is not BorrowedBookView rowData) return;

                var membersCols = new[] { "Member ID", "Member Name" };
                var bookCols = new[] { "Book ID", "Book title", "Book author" };

                if (membersCols.Contains(header))
                {
                    var memberRes = await MemberService.GetMemberAsync(FlowSide.Client, rowData.MemberID);
                    var member = memberRes.ActionResult ? memberRes.Data : null;
                    if (member == null) return;
                    var memberInfo = new MemberInfoWindow(member);
                    memberInfo.ShowDialog();
                    await LoadTablesAsync();
                }
                else if (bookCols.Contains(header))
                {
                    var bookRes = await BookService.GetBookByIdAsync(FlowSide.Client, rowData.BookID);
                    var book = bookRes.ActionResult ? bookRes.Data : null;
                    if (book == null) return;
                    var bookInfo = new BookInfoWindow(book);
                    bookInfo.ShowDialog();
                    await LoadTablesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addNewIssue = new AddIssueBook();
                if (addNewIssue.ShowDialog() != true) return;
                var bookIssue = addNewIssue.NewBorrow;
                if (bookIssue is not { Member: not null, Book: not null }) return;
                await BorrowService.IssueBookAsync(FlowSide.Client, bookIssue.Book.BookID, bookIssue.Member.MemberID, bookIssue.ExpectedReturnDate);
                await LoadTablesAsync();
                MessageBox.Show("Borrowed book successful", "Confirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }

        private void BtnExportHistory_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog.ExportWindow([..BorrowHistory]);
        }

        private void BtnExportCurrent_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog.ExportWindow([..CurrentBorrows]);
        }
    }
}
