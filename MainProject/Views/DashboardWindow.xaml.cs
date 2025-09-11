using System.Windows;
using LibrarySystemModels.Models;

namespace Library_System_Management.Views
{
    public partial class DashboardWindow : Window
    {
        internal User CurrentUser { get; }

        public DashboardWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
        }
        
        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
            BooksWindow bookWindow = new BooksWindow();
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

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            var users=new UsersWindow();
            users.ShowDialog();
        }
    }
}