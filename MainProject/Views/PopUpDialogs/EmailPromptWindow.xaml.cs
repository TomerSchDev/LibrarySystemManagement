using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views.PopUpDialogs
{
    public partial class EmailPromptWindow : Window
    {
        public string EnteredEmail => TxtEmail.Text.Trim();

        public EmailPromptWindow()
        {
            InitializeComponent();
            Loaded += EmailPromptWindow_Loaded;
        }

        private async void EmailPromptWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load member emails async
                var res = await MemberService.GetAllMembersAsync(FlowSide.Client);
                var memberEmails = res is { ActionResult: true, Data: not null }
                    ? res.Data.Where(m => !string.IsNullOrEmpty(m.Email) && !string.IsNullOrEmpty(m.FullName)).ToList()
                    : [];
                ComboEmails.ItemsSource = memberEmails;
                ComboEmails.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private void ComboEmails_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboEmails.SelectedItem is not Member member) return;
            TxtEmail.Text = member.Email;
            TxtEmail.Focus();
            TxtEmail.Select(TxtEmail.Text.Length, 0); // Place caret at end
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnteredEmail) || !MyRegex().IsMatch(EnteredEmail))
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
}
