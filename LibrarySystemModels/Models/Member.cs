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
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(Member)) return false;
        var member = obj as Member;
        return GetHashCode() == member.GetHashCode();
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(FullName);
        hashCode.Add(Email);
        hashCode.Add(Phone);
        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"Member full name: {FullName}, email: {Email}, phone: {Phone}";
    }
}