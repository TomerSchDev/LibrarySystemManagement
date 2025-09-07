using System.Collections.Generic;
using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Services;

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
    }
}