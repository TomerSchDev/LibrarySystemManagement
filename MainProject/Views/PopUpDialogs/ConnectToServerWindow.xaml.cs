using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarySystemModels;
using LibrarySystemModels.Database.Servers;
using LibrarySystemModels.Services;
using static System.Windows.Media.Color;

namespace Library_System_Management.Views.PopUpDialogs;

public partial class ConnectToServerWindow : Window
{
    public bool Connected = false;
    private string preAddress = "";
    public ConnectToServerWindow()
    {
        
        InitializeComponent();
        foreach (var obj in new[]{  RestApiServer.GetServer(), LocalApiSimulator.GetServer()})
        {
            var item = new ComboBoxItem();
            item.Content = obj.ServerTypeName(); // displayed text
            item.Tag     = obj;                  // reference to the original object, if you need it
            CmbServer.Items.Add(item);
        }
    }

    

    private async void  BtnConnect_Click(object sender, RoutedEventArgs e)
    {
        
        if (string.IsNullOrEmpty(TxtServerAdresss.Text)) return;
        if (TxtServerAdresss.Text.Equals(preAddress))return;
        Connected = false;
        if ((CmbServer.SelectedItem as ComboBoxItem)?.Tag is not DataServers server) return;

        LabelConnection.Content = "Connecting...";
        LabelConnection.Foreground = new SolidColorBrush(FromRgb(0,0,255));
        var res = await server.Connect(TxtServerAdresss.Text);
        if (res)
        {
            LabelConnection.Content = "Connected!";
            LabelConnection.Foreground = new SolidColorBrush(FromRgb(0,255,0));
            Connected = true;
        }
        else
        {
            LabelConnection.Content = "Failed to connect!";
            LabelConnection.Foreground= new SolidColorBrush(FromRgb(255,0,0));
        }

    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (_contentLoaded && CmbServer.SelectedItem is DataServers server) DataBaseService.SetDataServer(server);
    }
}