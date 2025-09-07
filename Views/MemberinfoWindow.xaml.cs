using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views
{
    public partial class MemberInfoWindow : Window
    {
        public Member SelectedMember { get; }
        public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; }
        public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; }

        public ICommand ReturnBookCommand { get; }
        public ICommand ExtendBorrowCommand { get; }
        public ICommand AddBorrowCommand { get; }

        public MemberInfoWindow(Member member)
        {
            InitializeComponent();
            SelectedMember = member;

            CurrentBorrows = new ObservableCollection<BorrowedBookView>();
            BorrowHistory = new ObservableCollection<BorrowedBookView>();
            CurrentBorrows.Clear();
            BorrowHistory.Clear();

            ReturnBookCommand = new RelayCommand<BorrowedBookView>(ReturnBook);
            ExtendBorrowCommand = new RelayCommand<BorrowedBookView>(ExtendBorrow);
            AddBorrowCommand = new RelayCommand(AddBorrow);

            DataContext = this;
        }

        private void loadBorrowHistory()
        {
            CurrentBorrows.Clear();
            BorrowHistory.Clear();
            var memberBorrows = BorrowService.GetBorrowHistory(SelectedMember.MemberID);
            CurrentBorrows = new ObservableCollection<BorrowedBookView>(
                memberBorrows.Where(b => !b.Returned)
            );
            BorrowHistory = new ObservableCollection<BorrowedBookView>(
                memberBorrows.Where(b => b.Returned)
            );;
            
        }

    

    private void ReturnBook(BorrowedBookView borrow)
    {
        if (MessageBox.Show("Return this book?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        BorrowService.ReturnBook(borrow.BorrowID);
        CurrentBorrows.Remove(borrow);
        BorrowHistory.Add(borrow); // move to history
    }

        private static void ExtendBorrow(BorrowedBookView borrow)
        {
            
            var laterDate=(DateTime.Today > borrow.ExpectedReturnDate ? DateTime.Today : borrow.ExpectedReturnDate) ??
                          DateTime.Now;
            borrow.ExpectedReturnDate=laterDate.AddDays(14) ;
        }

        private void AddBorrow()
        {
            // Show a dialog where librarian selects a book to borrow
            var window = new AddBorrowWindow(SelectedMember);
            if (window.ShowDialog() != true) return;
            
            var newBorrow = window.NewBorrow;
            if (newBorrow == null) return;
            BorrowService.IssueBook(newBorrow.BookID, SelectedMember.MemberID, newBorrow.ExpectedReturnDate);
            loadBorrowHistory();

        }
    }
}
