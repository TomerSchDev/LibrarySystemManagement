using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Library_System_Management.Views.PopUpDialogs;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Helpers;

namespace Library_System_Management.Views
{
    public partial class BooksWindow : Window
    {
        public BooksViewModel ViewModel { get; } = new();

        public BooksWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += async (s, e) => await ViewModel.LoadBooksAsync();
        }

        private void dgBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgBooks.SelectedItem is Book item)
                OpenBookWindow(item);
        }

        private async void OpenBookWindow(Book book)
        {
            try
            {
                var bookInfoWindow = new BookInfoWindow(book);
                bookInfoWindow.ShowDialog();
                await ViewModel.LoadBooksAsync();
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e);
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new AddEditBookWindow(null);
                if (addWindow.ShowDialog() != true)
                {
                    MessageBox.Show($"Couldn't add new Book", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var b = addWindow.GetEditingBook();
                if (b == null)
                {
                    MessageBox.Show($"Couldn't add new Book", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                b.Available = b.Quantity;
                await BookService.AddBookAsync(FlowSide.Client, b);
                await ViewModel.LoadBooksAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgBooks.SelectedItem is Book selected)
                {
                    var editWindow = new AddEditBookWindow(selected);
                    if (editWindow.ShowDialog() != true)
                    {
                        MessageBox.Show($"Couldn't edit Book " + selected.Title, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var b = editWindow.GetEditingBook();
                    if (b == null)
                    {
                        MessageBox.Show($"Couldn't edit Book " + selected.Title, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var listBorrowedRes = await BorrowService.GetBorrowHistoryByBookIdAsync(FlowSide.Client, b.BookID);
                    var borrowedCount = listBorrowedRes.ActionResult ? listBorrowedRes.Data.Count : 0;
                    b.Available = Math.Max(b.Quantity - borrowedCount, 0);

                    await BookService.UpdateBookAsync(FlowSide.Client, b);
                    await ViewModel.LoadBooksAsync();
                }
                else
                {
                    MessageBox.Show("Select a book to edit.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgBooks.SelectedItem is Book selected)
                {
                    await BookService.DeleteBookAsync(FlowSide.Client, selected.BookID);
                    await ViewModel.LoadBooksAsync();
                }
                else
                {
                    MessageBox.Show("Select a book to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (DgBooks.SelectedItem is Book selected)
                OpenBookWindow(selected);
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await BookService.GetAllBooksAsync(FlowSide.Client);
                if (result.ActionResult)
                    ExportDialog.ExportWindow([..result.Data]);
                else
                    MessageBox.Show("Failed to export: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        // ------ INNER VIEWMODEL --------
        public class BooksViewModel : INotifyPropertyChanged
        {
            private ObservableCollection<Book> _books = new();
            public ObservableCollection<Book> Books
            {
                get => _books;
                set { _books = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

            public async Task LoadBooksAsync()
            {
                var result = await BookService.GetAllBooksAsync(FlowSide.Client);
                Console.WriteLine("Books loaded");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();
                    if (result.ActionResult)
                    {
                        foreach (var book in result.Data)
                            Books.Add(book);
                    }
                    else
                    {
                        MessageBox.Show("Failed to load books: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }
    }
}