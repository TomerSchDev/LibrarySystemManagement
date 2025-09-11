using System.Windows;
using LibrarySystemModels.Models;

namespace Library_System_Management.Views.PopUpDialogs;

public partial class AddEditMemberWindow : Window
{
    public AddEditMemberWindow(Member? member)
    {
        InitializeComponent();
        _mMember = member;
        if  (member == null) return;
        TxtName.Text = _mMember!.FullName;
        TxtEmail.Text=_mMember!.Email;
        TxtPhone.Text=_mMember!.Phone;
        
    }

    private Member? _mMember;

    public Member? GetMember()
    {
        return _mMember;}

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        var textBoxes=  new []{TxtName,TxtEmail,TxtPhone};
        if (textBoxes.Any(textBox => textBox.Text == string.Empty && _mMember == null))
        {
            MessageBox.Show("One of entire is empty for new member");
            return;
        }

        var tmp = new Member(
            TxtName.Text != string.Empty ? TxtName.Text : _mMember!.FullName,
            TxtEmail.Text != string.Empty ? TxtEmail.Text : _mMember!.Email,
            TxtPhone.Text != string.Empty ? TxtPhone.Text : _mMember!.Phone);

        _mMember = tmp;
        DialogResult = true;
        Close();
    }
}