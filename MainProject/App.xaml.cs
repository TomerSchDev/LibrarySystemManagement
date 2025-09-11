using System.Windows;
using LibrarySystemModels.Services;

namespace Library_System_Management
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DataBaseService.SetModes(false, false);
            DataBaseService.Init(null);
            base.OnStartup(e);
        }
    }
}