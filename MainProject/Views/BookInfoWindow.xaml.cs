using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;
using System.Collections.Generic;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views
{
    public partial class BookInfoWindow : Window
    {
        public Book SelectedBook { get; }
        public ObservableCollection<BorrowedBookView> CurrentBorrows { get; set; } = new();
        public ObservableCollection<BorrowedBookView> BorrowHistory { get; set; } = new();
        public ICommand MemberInfoCommand { get; }

        public BookInfoWindow(Book b)
        {
            SelectedBook = b;
            InitializeComponent();
            MemberInfoCommand = new RelayCommand(OpenMemberInfoWindow);
            DataContext = this;
            Loaded += BookInfoWindow_Loaded;
        }

        private async void BookInfoWindow_Loaded(object sender, RoutedEventArgs e)
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
                var result = await BorrowService.GetBorrowHistoryByBookIdAsync(FlowSide.Client, SelectedBook.BookID);
                if (result.ActionResult)
                {
                    foreach (var b in result.Data)
                    {
                        // Console.WriteLine(b.ToString());
                        if (b.Returned)
                            BorrowHistory.Add(b);
                        else
                            CurrentBorrows.Add(b);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load borrows: " + (result.Message ?? ""), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrow history:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OpenMemberInfoWindow(object? param)
        {
            try
            {
                if (param is not BorrowedBookView borrow) return;

                var memberResult = await MemberService.GetMemberAsync(FlowSide.Client, borrow.MemberID);
                var member = memberResult.ActionResult ? memberResult.Data : null;
                if (member == null) return;

                var memberInfo = new MemberInfoWindow(member);
                memberInfo.ShowDialog();

                await LoadTablesAsync(); // Reload in case the member status/data changed
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog.ExportWindow([SelectedBook]);
        }

        private void BtnExportHistory_Click(object sender, RoutedEventArgs e)
        {
            var data = new List<BorrowedBookView>();
            data.AddRange(BorrowHistory);
            data.AddRange(CurrentBorrows);
            ExportDialog.ExportWindow([..data]);
        }
    }
}
