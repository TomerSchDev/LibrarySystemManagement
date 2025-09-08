using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class IssueReturnWindow : Window
{
    public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; }
    public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; }

    public IssueReturnWindow()
    {
        InitializeComponent();
        CurrentBorrows = new ObservableCollection<BorrowedBookView>();
        BorrowHistory = new ObservableCollection<BorrowedBookView>();
        LoadTables();
        DataContext = this;
    }

    private void LoadTables()
    {
        CurrentBorrows.Clear();
        BorrowHistory.Clear();
        var borrowRecords = BorrowService.GetBorrowHistoryEveryThing();
        foreach (var record in borrowRecords)
        {
            if (record.Returned)
            {
                BorrowHistory.Add(record);
            }
            else
            {
                CurrentBorrows.Add(record);
            }
        }
        

    }

    private void tbl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var depObj = (DependencyObject)e.OriginalSource;
        while (depObj != null && !(depObj is DataGridCell))
            depObj = VisualTreeHelper.GetParent(depObj);
        var cell = depObj as DataGridCell;
        if (cell == null) return;
        // Column index
        // Optionally, get the header
        var header = cell.Column.Header?.ToString() ?? "";
        if (header == "") return;
        if (cell.DataContext is not BorrowedBookView rowData) return;
        var membersCols = new[] { "Member ID", "Member Name" };
        var bookCols = new[] { "Book ID", "Book title", "Book author" };
        if (membersCols.Contains(header))
        {
            var member = rowData.Member;
            if (member == null) return;
            var memberInfo = new MemberInfoWindow(member);
            memberInfo.ShowDialog();
            LoadTables();
        }
        else if (bookCols.Contains(header))
        {
            var book = rowData.Book;
            if (book == null) return;
            var bookInfo = new BookInfoWindow(book);
            bookInfo.ShowDialog();
            LoadTables();
        }
    }


    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var addNewIssue = new AddIssueBook();
        if (addNewIssue.ShowDialog()!= true) return;
        var bookIssue = addNewIssue.NewBorrow;
        if (bookIssue is { Member: not null, Book: not null }) BorrowService.IssueBook(bookIssue.Book, bookIssue.Member, bookIssue.ReturnDate);
        LoadTables();
        MessageBox.Show("Borrowed book successes", "Confirm", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}