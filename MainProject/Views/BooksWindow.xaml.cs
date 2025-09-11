using System.Windows;
using System.Windows.Input;
using LibrarySystemModels.Services;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views
{
    public partial class BooksWindow : Window
    {
        public BooksWindow()
        {
            InitializeComponent();
            Loaded += BooksWindow_Loaded;
        }

        private async void BooksWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadBooksAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async Task LoadBooksAsync()
        {
            var result = await BookService.GetAllBooksAsync(FlowSide.Client);
            if (result.ActionResult)
                this.dgBooks.ItemsSource = result.Data;
            else
                MessageBox.Show("Failed to load books: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void dgBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgBooks.SelectedItem is Book item)
                OpenBookWindow(item);
        }

        private async void OpenBookWindow(Book book)
        {
            try
            {
                var bookInfoWindow = new BookInfoWindow(book);
                bookInfoWindow.ShowDialog();
                await LoadBooksAsync();
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
                await LoadBooksAsync();
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
                if (dgBooks.SelectedItem is Book selected)
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
                    await LoadBooksAsync();
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
                if (dgBooks.SelectedItem is Book selected)
                {
                    await BookService.DeleteBookAsync(FlowSide.Client, selected.BookID);
                    await LoadBooksAsync();
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
            if (dgBooks.SelectedItem is Book selected)
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
    }
}
