using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Services;

public static class LocalApiSimulator
{
    private static User _CurrentUser;
    // INSERT (POST)
    public static async Task<ResultResolver<TResult>?> InsertAsync<TResult, TPayload>(string url, TPayload data)
    {
        return await Task.Run(async () =>
        {
            // Books
            if (url.StartsWith("api/Books", StringComparison.OrdinalIgnoreCase) && data is Book book)
                return await BookService.AddBookAsync(FlowSide.Server, book) as ResultResolver<TResult>;

            // Members
            if (url.StartsWith("api/Members", StringComparison.OrdinalIgnoreCase) && data is Member member)
                return await MemberService.AddMemberAsync(FlowSide.Server, member) as ResultResolver<TResult>;

            // Reports
            if (url.StartsWith("api/Reports", StringComparison.OrdinalIgnoreCase) && data is Report report)
                return await ReportingService.AddReportAsync(FlowSide.Server, report) as ResultResolver<TResult>;

            // BorrowedBooks (IssueBookDto)
            if (url.StartsWith("api/BorrowedBooks/issue", StringComparison.OrdinalIgnoreCase) && data is IssueBookDto dto)
                return await BorrowService.IssueBookAsync(FlowSide.Server, dto.BookId, dto.MemberId, dto.ReturnDate) as ResultResolver<TResult>;

            throw new NotImplementedException($"[LocalApiSimulator.InsertAsync] Route not mapped: {url}, type {typeof(TPayload).Name}");
        });
    }

    // UPDATE (PUT)
    public static async Task<ResultResolver<TResult>?> UpdateAsync<TResult, TPayload>(string url, TPayload data)
    {
        return await Task.Run(async () =>
        {
            // Books
            if (url.StartsWith("api/Books", StringComparison.OrdinalIgnoreCase) && data is Book book)
                return await BookService.UpdateBookAsync(FlowSide.Server, book) as ResultResolver<TResult>;

            // Members
            if (url.StartsWith("api/Members", StringComparison.OrdinalIgnoreCase) && data is Member member)
                return await MemberService.UpdateMemberAsync(FlowSide.Server, member) as ResultResolver<TResult>;

            // Reports
            if (url.StartsWith("api/Reports", StringComparison.OrdinalIgnoreCase) && data is Report report)
                return await ReportingService.UpdateReportAsync(FlowSide.Server, report) as ResultResolver<TResult>;

            // BorrowedBooks (Return)
            if (url.StartsWith("api/BorrowedBooks/return", StringComparison.OrdinalIgnoreCase))
            {
                int borrowId = ParseId(url);
                return await BorrowService.ReturnBookAsync(FlowSide.Server, borrowId) as ResultResolver<TResult>;
            }

            throw new NotImplementedException($"[LocalApiSimulator.UpdateAsync] Route not mapped: {url}, type {typeof(TPayload).Name}");
        });
    }

    // DELETE
    public static async Task<ResultResolver<TResult>?> DeleteAsync<TResult>(string url)
    {
        return await Task.Run(async () =>
        {
            // Books
            if (url.StartsWith("api/Books", StringComparison.OrdinalIgnoreCase))
            {
                var id = ParseId(url);
                return await BookService.DeleteBookAsync(FlowSide.Server, id) as ResultResolver<TResult>;
            }
            // Members
            if (url.StartsWith("api/Members", StringComparison.OrdinalIgnoreCase))
            {
                int id = ParseId(url);
                return await MemberService.DeleteMemberAsync(FlowSide.Server, id) as ResultResolver<TResult>;
            }
            // Reports
            if (url.StartsWith("api/Reports", StringComparison.OrdinalIgnoreCase))
            {
                int id = ParseId(url);
                return await ReportingService.DeleteReportAsync(FlowSide.Server, id) as ResultResolver<TResult>;
            }
            throw new NotImplementedException($"[LocalApiSimulator.DeleteAsync] Route not mapped: {url}");
        });
    }

    // GET
    public static async Task<ResultResolver<TResult>?> GetAsync<TResult>(string url)
    {
        return await Task.Run(async () =>
        {

            // Books (all)
            if (url.Equals("api/Books/", StringComparison.OrdinalIgnoreCase))
                return await BookService.GetAllBooksAsync(FlowSide.Server) as ResultResolver<TResult>;

            // Book (by id)
            if (url.StartsWith("api/Books/", StringComparison.OrdinalIgnoreCase))
            {
                int bookId = ParseId(url);
                return await BookService.GetBookByIdAsync(FlowSide.Server, bookId) as ResultResolver<TResult>;
            }

            // Members (all)
            if (url.Equals("api/Members/", StringComparison.OrdinalIgnoreCase))
                return await MemberService.GetAllMembersAsync(FlowSide.Server) as ResultResolver<TResult>;

            // Member (by id)
            if (url.StartsWith("api/Members/", StringComparison.OrdinalIgnoreCase))
            {
                int memberId = ParseId(url);
                return await MemberService.GetMemberAsync(FlowSide.Server, memberId) as ResultResolver<TResult>;
            }

            // Reports (all)
            if (url.Equals("api/Reports/", StringComparison.OrdinalIgnoreCase))
                return await ReportingService.GetReportsAsync(FlowSide.Server) as ResultResolver<TResult>;

            // Reports by user
            if (url.StartsWith("api/Reports/user/", StringComparison.OrdinalIgnoreCase))
            {
                var userId = ParseId(url);
                var user = await AuthService.GetUserByIdAsync(FlowSide.Server, userId);
                return await ReportingService.GetReportsByUserAsync(FlowSide.Server,
                    user ?? AuthService.CreateUser("Default user", "pass", UserRole.Admin)) as ResultResolver<TResult>;
            }

            // BorrowedBooks history by member
            if (url.StartsWith("api/BorrowedBooks/history/member/", StringComparison.OrdinalIgnoreCase))
            {
                int memberId = ParseId(url);
                return await BorrowService.GetBorrowHistoryByMemberIdAsync(FlowSide.Server, memberId) as
                    ResultResolver<TResult>;
            }

            // BorrowedBooks history by book
            if (url.StartsWith("api/BorrowedBooks/history/book/", StringComparison.OrdinalIgnoreCase))
            {
                int bookId = ParseId(url);
                return await BorrowService.GetBorrowHistoryByBookIdAsync(FlowSide.Server, bookId) as
                    ResultResolver<TResult>;
            }

            // BorrowedBooks all history
            if (url.Equals("api/BorrowedBooks/history/all", StringComparison.OrdinalIgnoreCase))
                return await BorrowService.GetBorrowHistoryEveryThingAsync(FlowSide.Server) as ResultResolver<TResult>;

            if (url.Equals("api/Auth/current", StringComparison.CurrentCulture))
            {
                return await Task.Run(() =>
                    new ResultResolver<User>(_CurrentUser, !User.IsDefaultUser(_CurrentUser), "") as
                        ResultResolver<TResult>);
            }

            return null;
        });
    }

    // Utility to extract last int component from URL
    private static int ParseId(string url)
    {
        var parts = url.TrimEnd('/').Split('/');
        return int.TryParse(parts[^1], out var id) ? id : -1;
    }

    // LOGIN stub (add your actual logic)
    public static async Task<ResultResolver<User>> LoginAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            var user =  AuthService.LoginAsync(FlowSide.Server, username, password).Result;
            _CurrentUser = user;
            return User.IsDefaultUser(user) ? new ResultResolver<User>(user,false,"") : new ResultResolver<User>(user,true,"");
        });
    }
}

// DTO for local borrowed book issue (make sure it matches your project)
internal class IssueBookDto
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime? ReturnDate { get; set; }
}