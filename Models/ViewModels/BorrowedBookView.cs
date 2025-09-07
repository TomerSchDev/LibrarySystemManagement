namespace Library_System_Management.Models.ViewModels

{
    public class BorrowedBookView
    {
        public int BorrowID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }

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
    }
}
