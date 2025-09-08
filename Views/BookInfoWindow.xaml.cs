using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class BookInfoWindow : Window
{
    public Book SelectedBook { get; }
    public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; }
    public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; }
    public ICommand MemberInfoCommend { get; }
    public BookInfoWindow(Book b)
    {
        SelectedBook = b;
        CurrentBorrows = [];
        BorrowHistory = [];
        LoadTables();
        InitializeComponent();
        MemberInfoCommend = new RelayCommand<BorrowedBookView>(OpenMemberInfoWindow);
        DataContext = this;


    }

    private void OpenMemberInfoWindow(BorrowedBookView borrow)
    {
        var memberId=borrow.MemberID;
        var member=MemberService.GetMember(memberId);
        if (member == null) return;
        var memberInfo = new MemberInfoWindow(member);
        memberInfo.Show();
        LoadTables();
    }
    private void LoadTables()
    {
        CurrentBorrows.Clear();
        BorrowHistory.Clear();
        var borrowList = BorrowService.GetBorrowHistoryByBookId(SelectedBook.BookID);
        foreach (var b in borrowList)
        {
            Console.WriteLine(b.toString());
            if (b.Returned)
            {
                BorrowHistory.Add(b);
            }

            else
            {
                CurrentBorrows.Add(b);
            }
        }

    }
}