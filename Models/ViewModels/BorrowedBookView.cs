namespace Library_System_Management.Models.ViewModels

{
    public class BorrowedBookView : IExportable
    {
        public int BorrowID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }
        public Member? Member { get; set; }
        public Book? Book { get; set; }   // linked Book object

        public DateTime BorrowDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool Returned { get; set; }

        // Computed property for UI
        public string Status
        {
            get
            {
                if (!Returned) return "Not Returned";
                if (ReturnDate == null) return "Not Returned";
                return ReturnDate <= ExpectedReturnDate ? "On Time" : "Late";
            }
        }

        public string toString()
        {
            return $"Borrow record ID: {BorrowID},  Book Id {BookID}, Book Title {Book?.Title}  Member Id {MemberID}, Member Name {Member?.FullName} Status {Status}";
        }

        public string ExportClassName => "BorrowedBookView";
    }
}
