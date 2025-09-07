using System.Windows;
using Library_System_Management.Models;

namespace Library_System_Management.Views
{
    public partial class DashboardWindow : Window
    {
        private User currentUser;

        public DashboardWindow(User user)
        {
            InitializeComponent();
            currentUser = user;

            // Optional: restrict admin-only buttons
            if(currentUser.UserType != "Admin")
            {
                // Only allow members to view books and issue books
            }
        }

        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
            BookWindow bookWindow = new BookWindow();
            bookWindow.ShowDialog();
        }

        private void BtnMembers_Click(object sender, RoutedEventArgs e)
        {
            MembersWindow memberWindow = new MembersWindow();
            memberWindow.ShowDialog();
        }

        private void BtnIssueReturn_Click(object sender, RoutedEventArgs e)
        {
            IssueReturnWindow issueReturnWindow = new IssueReturnWindow();
            issueReturnWindow.ShowDialog();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow();
            reportsWindow.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search=new  SearchWindow();
            search.ShowDialog();
            
        }
    }
}