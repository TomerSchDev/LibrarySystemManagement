using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Services;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;

namespace Library_System_Management.Views
{
    public partial class MemberInfoWindow : Window
    {
        public Member SelectedMember { get; }

        public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; } = new();
        public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; } = new();

        public ICommand ReturnBookCommand { get; }
        public ICommand ExtendBorrowCommand { get; }
        public ICommand AddBorrowCommand { get; }

        public MemberInfoWindow(Member member)
        {
            InitializeComponent();
            SelectedMember = member;

            ReturnBookCommand = new RelayCommand(param =>
            {
                if (param is BorrowedBookView borrow)
                    ReturnBook(borrow);
            });
            ExtendBorrowCommand = new RelayCommand(param =>
            {
                if (param is BorrowedBookView borrow)
                    ExtendBorrow(borrow);
            });
            AddBorrowCommand = new RelayCommand(_ => AddBorrow());

            DataContext = this;
            Loaded += async (_, _) => await LoadBorrowHistoryAsync();
        }

        private async Task LoadBorrowHistoryAsync()
        {
            CurrentBorrows.Clear();
            BorrowHistory.Clear();
            var memberBorrowsResult = await BorrowService.GetBorrowHistoryByMemberIdAsync(FlowSide.Client, SelectedMember.MemberID);
            if (memberBorrowsResult is { ActionResult: true, Data: not null })
            {
                foreach (var borrow in memberBorrowsResult.Data)
                {
                    if (!borrow.Returned)
                        CurrentBorrows.Add(borrow);
                    else
                        BorrowHistory.Add(borrow);
                }
            }
            else
            {
                MessageBox.Show("Failed to load borrows: " + memberBorrowsResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ReturnBook(BorrowedBookView borrow)
        {
            if (MessageBox.Show("Return this book?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            await BorrowService.ReturnBookAsync(FlowSide.Client, borrow.BorrowID);
            CurrentBorrows.Remove(borrow);
            BorrowHistory.Add(borrow);
        }

        private static async void ExtendBorrow(BorrowedBookView borrow)
        {
            try
            {
                var laterDate = (DateTime.Today > borrow.ExpectedReturnDate ? DateTime.Today : borrow.ExpectedReturnDate) ?? DateTime.Now;
                var newDate = laterDate.AddDays(BorrowedBook.ExtendDays);
                var span = newDate.Subtract(borrow.ExpectedReturnDate!.Value);
                await BorrowService.ExtendBookAsync(FlowSide.Client,borrow.BorrowID,span.Days);
                MessageBox.Show("Extant book successfully");
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e);
            }
        }

        private async void AddBorrow()
        {
            try
            {
                var window = new AddBorrowWindow(SelectedMember);
                if (window.ShowDialog() != true) return;
                var newBorrow = window.NewBorrow;
                if (newBorrow?.Book == null) return;

                await BorrowService.IssueBookAsync(FlowSide.Client, newBorrow.Book.BookID, SelectedMember.MemberID, newBorrow.ExpectedReturnDate);
                await LoadBorrowHistoryAsync();
                MessageBox.Show("Borrowed book successes", "Confirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e);
            }
        }

        private void BtnExportMember_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog.ExportWindow([SelectedMember]);
        }

        private void BtnExportBorrow_Click(object sender, RoutedEventArgs e)
        {
            var data = new List<BorrowedBookView>(CurrentBorrows);
            data.AddRange(BorrowHistory);
            ExportDialog.ExportWindow([..data]);
        }
    }
}
