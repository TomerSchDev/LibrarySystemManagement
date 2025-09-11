using System.Collections.ObjectModel;
using System.Windows;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Models;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;

namespace Library_System_Management.Views
{
    public partial class MembersWindow : Window
    {
        // Use ObservableCollection for live UI updates!
        private ObservableCollection<Member> Members { get; set; } = [];

        public MembersWindow()
        {
            InitializeComponent();
            Loaded += MembersWindow_Loaded;
        }

        private async void MembersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadMembersAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async Task LoadMembersAsync()
        {
            var membersResult = await MemberService.GetAllMembersAsync(FlowSide.Client);
            Members.Clear();
            if (membersResult is { ActionResult: true, Data: not null })
            {
                foreach (var m in membersResult.Data)
                    Members.Add(m);
            }
            else
            {
                MessageBox.Show("Failed to load members: " + membersResult.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DgMembers.ItemsSource = Members;
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new AddEditMemberWindow(null);
                addWindow.ShowDialog();

                if (addWindow.DialogResult != true)
                {
                    MessageBox.Show("Couldn't add member");
                    return;
                }

                var member = addWindow.GetMember();
                if (member == null)
                {
                    MessageBox.Show("Couldn't add member");
                    return;
                }

                await MemberService.AddMemberAsync(FlowSide.Client, member);
                await LoadMembersAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (DgMembers.SelectedItem is not Member selected)
            {
                MessageBox.Show("Couldn't select member");
                return;
            }

            var memberInfoWindow = new MemberInfoWindow(selected);
            memberInfoWindow.ShowDialog();
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgMembers.SelectedItem is Member selected)
                {
                    var editWindow = new AddEditMemberWindow(selected);
                    if (editWindow.ShowDialog() != true)
                    {
                        MessageBox.Show($"Couldn't edit member {selected.FullName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var m = editWindow.GetMember();
                    if (m == null)
                    {
                        MessageBox.Show($"Couldn't edit member {selected.FullName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    await MemberService.UpdateMemberAsync(FlowSide.Client, m);
                    await LoadMembersAsync();
                }
                else
                {
                    MessageBox.Show("Select a Member to edit.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgMembers.SelectedItem is Member selected)
                {
                    await MemberService.DeleteMemberAsync(FlowSide.Client, selected.MemberID);
                    await LoadMembersAsync();
                }
                else
                {
                    MessageBox.Show("Select a member to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var membersResult = await MemberService.GetAllMembersAsync(FlowSide.Client);
                if (membersResult.ActionResult)
                    ExportDialog.ExportWindow([..membersResult.Data]);
                else
                    MessageBox.Show("Failed to export: " + membersResult.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);

            }
        }
    }
}
