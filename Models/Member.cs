namespace Library_System_Management.Models;

public class Member()
{
    public Member(string NameText, string EmailText, string PhoneText) : this()
    {
        MemberID = -1;
        this.FullName = NameText;
        this.Email = EmailText;
        this.Phone = PhoneText;
    }

    public int MemberID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}