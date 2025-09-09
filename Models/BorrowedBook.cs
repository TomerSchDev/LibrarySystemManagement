namespace Library_System_Management.Models;

public class BorrowedBook : IExportable
{
    public int BorrowId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public bool Returned { get; set; }
    public string ExportClassName =>"BorrowedBook";
}