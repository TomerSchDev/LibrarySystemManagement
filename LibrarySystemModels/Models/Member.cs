namespace LibrarySystemModels.Models;

public class Member() : IExportable
{
    public Member(string NameText, string EmailText, string PhoneText) : this()
    {
        MemberID = -1;
        FullName = NameText;
        Email = EmailText;
        Phone = PhoneText;
    }
    public int MemberID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ExportClassName =>"Member";
}