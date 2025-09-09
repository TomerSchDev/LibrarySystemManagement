using System.Windows;
using static System.Text.RegularExpressions.Regex;

namespace Library_System_Management.Views.PopUpDIalogs;

public partial class EmailPromptWindow : Window
{
    public string EnteredEmail => TxtEmail.Text.Trim();

    public EmailPromptWindow()
    {
        InitializeComponent();
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        // Quick email format validation (optional)
        if (string.IsNullOrWhiteSpace(EnteredEmail) ||
            !MyRegex().IsMatch(EnteredEmail))
        {
            MessageBox.Show("Please enter a valid email address.", "Validation",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtEmail.Focus();
            return;
        }
        DialogResult = true;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}