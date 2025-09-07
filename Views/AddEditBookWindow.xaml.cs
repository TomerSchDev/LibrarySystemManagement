using System;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Views
{
    public partial class AddEditBookWindow : Window
    {
        private Book? editingBook = null!;
        
        public AddEditBookWindow(Book? book)
        {
            InitializeComponent();
            if (book == null) return;
            editingBook = book;
            TxtTitle.Text = editingBook.Title;
            TxtAuthor.Text = editingBook.Author;
            TxtIsbn.Text = editingBook.ISBN;
            TxtQuantity.Text = editingBook.Quantity.ToString();
        }
        public Book? GetEditingBook(){return editingBook;}

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var textBoxes=  new []{TxtTitle,TxtAuthor,TxtIsbn,TxtQuantity};
            if (textBoxes.Any(textBox => textBox.Text == string.Empty && editingBook == null))
            {
                MessageBox.Show("One of entire is empty for new book");
                return;
            }

            var tmp = new Book(TxtTitle.Text != string.Empty ? TxtTitle.Text : editingBook!.Title,
                TxtAuthor.Text != string.Empty ? TxtAuthor.Text : editingBook!.Author,
                TxtIsbn.Text != string.Empty ? TxtIsbn.Text : editingBook!.ISBN,
                TxtQuantity.Text != string.Empty ? int.Parse(TxtQuantity.Text) : editingBook!.Quantity);

            editingBook = tmp;
            DialogResult = true;
            Close();
        }
    }
}