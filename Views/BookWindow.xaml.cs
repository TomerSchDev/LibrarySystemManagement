using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;
using Library_System_Management.Services.ExportServices;
using Library_System_Management.Views;

namespace Library_System_Management.Views
{
    public partial class BookWindow : Window
    {
        private List<Book> books;
        public BookWindow()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            books = BookService.GetAllBooks();
            dgBooks.ItemsSource = books;
        }

        private void OpenBookWindow(Book book)
        {
            var bookInfoWindow= new BookInfoWindow(book);
            bookInfoWindow.ShowDialog();
            LoadBooks();
        }

        private void dgBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid=sender as DataGrid;
            if (grid?.SelectedItem is not Book item) return;
            OpenBookWindow(item);
        }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditBookWindow(null);
            if (addWindow.ShowDialog() != true)
            {
                MessageBox.Show($"couldn't add new Book" , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var b = addWindow.GetEditingBook();
            if (b == null)
            {
                MessageBox.Show($"couldn't add new Book", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            b.Available = b.Quantity;
            BookService.AddBook(b);
            LoadBooks();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(dgBooks.SelectedItem is Book selected)
            {
                var editWindow = new AddEditBookWindow(selected);
                if (editWindow.ShowDialog() != true)
                {
                    MessageBox.Show($"couldn't edit Book "+selected.Title , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                var b = editWindow.GetEditingBook();
                if (b == null)
                {
                    MessageBox.Show($"couldn't edit Book "+selected.Title, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var listBorrowed = BorrowService.GetBorrowHistoryByBookId(b.BookID);
                b.Available = int.Max(b.Quantity-listBorrowed.Count,0);
                BookService.UpdateBook(b);
            }
            else
            {
                MessageBox.Show("Select a book to edit.");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(dgBooks.SelectedItem is Book selected)
            {
                BookService.DeleteBook(selected.BookID);
                LoadBooks();
            }
            else
            {
                MessageBox.Show("Select a book to delete.");
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem is not Book selected) return;
            OpenBookWindow(selected);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportDialog.ExportWindow([..BookService.GetAllBooks()]);
        }
        
    }

}