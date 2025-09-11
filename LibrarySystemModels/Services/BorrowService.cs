using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;

namespace LibrarySystemModels.Services
{
    public static class BorrowService
    {
        private const string BorrowServiceUrl = "api/Borrow/";

        public static async Task<ResultResolver<BorrowedBook>> IssueBookAsync(FlowSide side, int bookId, int memberId, DateTime? returnDate)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to issue books!";
                if (side != FlowSide.Client) return new ResultResolver<BorrowedBook>(null!, false, errorMessage);
                
                await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<BorrowedBook>(null!, false, errorMessage);
            }

            if (side == FlowSide.Client)
            {
                var dto = new IssueBookDto { BookId = bookId, MemberId = memberId, ReturnDate = returnDate };
                var res = await DataBaseService.Insert<BorrowedBook,IssueBookDto>(BorrowServiceUrl + "issue", dto);
                return new ResultResolver<BorrowedBook>(new BorrowedBook(), res.ActionResult, res.Message);
            }

            // Server logic (local DB)
            var borrowsRes = await GetBorrowHistoryByMemberIdAsync(side, memberId);
            var borrows = borrowsRes.Data ;
            if (borrows.Any(b => b.BookID == bookId && !b.Returned))
                return new ResultResolver<BorrowedBook>(null!, false, "Already borrowed");

            var borrowedBook = new BorrowedBook
            {
                BookId = bookId,
                MemberId = memberId,
                IssueDate = DateTime.Now,
                ReturnDate = returnDate ?? DateTime.Now.AddDays(14),
                Returned = false
            };

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(borrowedBook));
                return new ResultResolver<BorrowedBook>(borrowedBook, true, "");
            }
            catch (Exception ex)
            {
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, $"Failed to issue book: {ex.Message}");
                return new ResultResolver<BorrowedBook>(null!, false, "Database error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<BorrowedBook>> ReturnBookAsync(FlowSide side, int borrowId)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to return books!";
                if (side != FlowSide.Client) return new ResultResolver<BorrowedBook>(null!, false, errorMessage);
                
                await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<BorrowedBook>(null!, false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Update<BorrowedBook,BorrowedBook>(BorrowServiceUrl + $"return/{borrowId}", new BorrowedBook());

            // Server logic (local DB)
            var handler = DataBaseService.GetLocalDatabase();

            var borrows = await Task.Run(() => handler.SelectAll<BorrowedBook>());
            var borrow = borrows.FirstOrDefault(b => b.BorrowId == borrowId);

            if (borrow == null)
                return new ResultResolver<BorrowedBook>(null!, false, "Borrow record not found");

            if (borrow.Returned)
                return new ResultResolver<BorrowedBook>(borrow, false, "Already returned");

            borrow.Returned = true;
            borrow.ReturnedDate = DateTime.Now;
            try
            {
                await Task.Run(() => handler.Update(borrow));
                return new ResultResolver<BorrowedBook>(borrow, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<BorrowedBook>(borrow, false, "Error updating: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<BorrowedBook>> ExtendBookAsync(FlowSide side, int borrowId,
            int daysLater)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to extend Borrow book!";
                if (side != FlowSide.Client) return new(null!, false, errorMessage);
                
                await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<BorrowedBook>(null!, false, errorMessage);
            }

            if (side == FlowSide.Client)
                    return await DataBaseService.Update<BorrowedBook,Models.IssueBookDto>(BorrowServiceUrl + $"extend/{borrowId}",new Models.IssueBookDto() {BookId = borrowId,MemberId = daysLater,ReturnDate = DateTime.Now});
            // Server logic (local DB)
            var handler = DataBaseService.GetLocalDatabase();

            var borrows = await Task.Run(() => handler.SelectAll<BorrowedBook>());
            var borrow = borrows.FirstOrDefault(b => b.BorrowId == borrowId);

            if (borrow == null)
                return new ResultResolver<BorrowedBook>(null!, false, "Borrow record not found");

            if (borrow.Returned)
                return new ResultResolver<BorrowedBook>(borrow, false, "Already returned");

            borrow.ReturnDate = borrow.ReturnDate?.AddDays(daysLater);
            borrow.ReturnedDate = DateTime.Now;
            try
            {
                await Task.Run(() => handler.Update(borrow));
                return new ResultResolver<BorrowedBook>(borrow, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<BorrowedBook>(borrow, false, "Error updating: " + ex.Message);
            }
        }
        public static async Task<ResultResolver<List<BorrowedBookView>>> GetBorrowHistoryByMemberIdAsync(FlowSide side, int memberId)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<BorrowedBookView>>(BorrowServiceUrl + $"history/member/{memberId}");

            return new ResultResolver<List<BorrowedBookView>>(
                await Task.Run(() => GetBorrowedBooksHistoryByCondition(side, b => b.MemberID == memberId)),
                true, ""
            );
        }

        public static async Task<ResultResolver<List<BorrowedBookView>>> GetBorrowHistoryByBookIdAsync(FlowSide side, int bookId)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<BorrowedBookView>>(BorrowServiceUrl + $"history/book/{bookId}");

            return new ResultResolver<List<BorrowedBookView>>(
                await Task.Run(() => GetBorrowedBooksHistoryByCondition(side, b => b.BookID == bookId)),
                true, ""
            );
        }

        public static async Task<ResultResolver<List<BorrowedBookView>>> GetBorrowHistoryEveryThingAsync(FlowSide side)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<BorrowedBookView>>(BorrowServiceUrl + $"history/all");

            return new ResultResolver<List<BorrowedBookView>>(
                await Task.Run(() => GetBorrowedBooksHistoryByCondition(side, _=>true)),
                true, ""
            );
        }

        private static List<BorrowedBookView> GetBorrowedBooksHistoryByCondition(FlowSide side, Func<BorrowedBookView, bool> condition)
        {
            var allBorrowedBooks = DataBaseService.GetLocalDatabase().SelectAll<BorrowedBook>();
            return allBorrowedBooks.Select(b => new BorrowedBookView
            {
                BorrowID = b.BorrowId,
                BookID = b.BookId,
                MemberID = b.MemberId,
                BorrowDate = b.IssueDate,
                ExpectedReturnDate = b.ReturnDate,
                ReturnDate = b.ReturnedDate,
                Returned = b.Returned,
                Book = BookService.GetBookByIdAsync(side, b.BookId).Result.Data, // Consider making BookService.GetBookByIdAsync truly async everywhere!
                Member = MemberService.GetMemberAsync(side, b.MemberId).Result.Data
            }).Where(condition).ToList();
        }

       
    }
}
