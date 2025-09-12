namespace LibrarySystemModels.Models;
public class IssueBookDto
{
    public IssueBookDto()
    {
       
    }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime? ReturnDate { get; set; }
}
public class BorrowedBook : IExportable
{
    public static readonly int ExtendDays = 14;
    public int BorrowedBookID { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public bool Returned { get; set; }
    public string ExportClassName =>"BorrowedBook";
}