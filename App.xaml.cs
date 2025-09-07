using System.Windows;
using Library_System_Management.Database;

namespace Library_System_Management
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize SQLite database and tables
            DatabaseManager.InitializeDatabase();
        }
    }
}