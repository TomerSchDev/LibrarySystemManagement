using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using Xunit;

namespace TestsLibrary
{
    public class BorrowServiceTests : IDisposable
    {

        public BorrowServiceTests()
        {
            Utils.setLocalTests(Path.Combine("Resources", $"test_{Guid.NewGuid()}.sqlite"));
            Utils.LogInUser(null,FlowSide.Client);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            // if (File.Exists(_dbTestPath)) File.Delete(_dbTestPath);
        }

        private async Task<Member> AddMemberAsync()
        {
            var member = new Member { FullName = "Borrow Tester", MemberID = 101 };
            var res = await MemberService.AddMemberAsync(FlowSide.Client, member);
            Assert.True(res.ActionResult);
            return res.Data;
        }

        private async Task<Book> AddBookAsync()
        {
            var book = new Book { Title = "Borrow Book", Author = "Borrow Author", ISBN = "111222333" };
            var result = await BookService.AddBookAsync(FlowSide.Client, book);
            Assert.True(result.ActionResult);
            return book;
        }

        [Fact]
        public async Task CanIssueAndReturnBook()
        {
            var member = await AddMemberAsync();
            var book = await AddBookAsync();
            var issueResult = await BorrowService.IssueBookAsync(FlowSide.Client, book.BookID, member.MemberID, DateTime.Now.AddDays(14));
            Assert.True(issueResult.ActionResult);

            var history = await BorrowService.GetBorrowHistoryByMemberIdAsync(FlowSide.Client, member.MemberID);
            Assert.Contains(history.Data, b => b.BookID == book.BookID);

            // Return
            var borrowEntry = history.Data.First(b => b.BookID == book.BookID);
            var returnResult = await BorrowService.ReturnBookAsync(FlowSide.Client, borrowEntry.BorrowID);
            Assert.True(returnResult.ActionResult);
        }

        [Fact]
        public async Task CannotIssueBookTwiceWithoutReturn()
        {
            var member = await AddMemberAsync();
            var book = await AddBookAsync();
            var firstIssue = await BorrowService.IssueBookAsync(FlowSide.Client, book.BookID, member.MemberID, DateTime.Now.AddDays(7));
            Assert.True(firstIssue.ActionResult);

            var secondIssue = await BorrowService.IssueBookAsync(FlowSide.Client, book.BookID, member.MemberID, DateTime.Now.AddDays(7));
            Assert.False(secondIssue.ActionResult);
        }
    }
}
