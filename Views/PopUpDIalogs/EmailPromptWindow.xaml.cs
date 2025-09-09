using System.Collections.Generic;
using System.Windows;
using Library_System_Management.Services;     // For MemberService
using System.Linq;
using Library_System_Management.Models;

namespace Library_System_Management.Views.PopUpDialogs;

public partial class EmailPromptWindow : Window
{
    public string EnteredEmail => TxtEmail.Text.Trim();

    public EmailPromptWindow()
    {
        InitializeComponent();

        // Get member emails and bind to ComboBox
        var memberEmails = MemberService
            .GetAllMembers()
            .Where(m => !string.IsNullOrEmpty(m.Email) && !string.IsNullOrEmpty(m.FullName))
            .ToList();

        ComboEmails.ItemsSource = memberEmails;
        ComboEmails.SelectedIndex = -1; // for empty selection
    }

    // Sync selection: Insert selected email into TxtEmail
    private void ComboEmails_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (ComboEmails.SelectedItem is not Member member) return;
        TxtEmail.Text = member.Email;
        TxtEmail.Focus();
        TxtEmail.Select(TxtEmail.Text.Length, 0); // Place caret at end
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        // Your existing validation logic...
        if (string.IsNullOrWhiteSpace(EnteredEmail) ||
            !MyRegex().IsMatch(EnteredEmail))
        {
            MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtEmail.Focus();
            return;
        }

        DialogResult = true;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}