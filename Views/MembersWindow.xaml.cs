using System.Collections.ObjectModel;
using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class MembersWindow : Window
{
    private List<Member> _members = [];
    public MembersWindow()
    {
        InitializeComponent();
        LoadMembers();
    }
    private void LoadMembers()
    {
        _members = MemberService.GetAllMembers();

        DgMembers.ItemsSource = new ObservableCollection<Member>(_members);
        DataContext = this;

    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new AddEditMemberWindow(null);
        addWindow.ShowDialog();
        if (addWindow.DialogResult != true)
        {
            MessageBox.Show("couldn't add member");
            return;
        }

        var member = addWindow.GetMember();
        if (member == null)
        {
            MessageBox.Show("couldn't add member");
            return;
        }
        MemberService.AddMember(member);
        LoadMembers();
}

    private void BtnSelect_Click(object sender, RoutedEventArgs e)
    {
        if (DgMembers.SelectedItem is not Member selected)
        {
            MessageBox.Show("couldn't select member");
            return;
        }
        
        var memberInfoWindow =  new MemberInfoWindow(selected);
        memberInfoWindow.ShowDialog();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        
        if(DgMembers.SelectedItem is Member selected)
        {
            var editWindow = new AddEditMemberWindow(selected);
            if (editWindow.ShowDialog() != true)
            {
                MessageBox.Show($"couldn't edit member "+selected.FullName , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var m = editWindow.GetMember();
            if (m == null)
            {
                MessageBox.Show($"couldn't edit member "+selected.FullName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MemberService.UpdateMember(m);
        }
        else
        {
            MessageBox.Show("Select a Member to edit.");
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if(DgMembers.SelectedItem is Member selected)
        {
            MemberService.DeleteMember(selected.MemberID);
            LoadMembers();
        }
        else
        {
            MessageBox.Show("Select a book to delete.");
        }
    }
}