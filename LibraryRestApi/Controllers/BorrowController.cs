using LibrarySystemModels.Models;
using LibrarySystemModels.Models.ViewModels;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryRestApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowController : ControllerBase
    {
        [HttpPost("issue")]
        public async Task<IActionResult> IssueBook([FromBody] IssueBookDto dto)
        {
            var result = await BorrowService.IssueBookAsync(FlowSide.Server, dto.BookId, dto.MemberId, dto.ReturnDate);
            return Ok(result);
        }

        [HttpPost("return/{borrowId:int}")]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var result = await BorrowService.ReturnBookAsync(FlowSide.Server, borrowId);
            return Ok(result);
        }
        [HttpPost("extend/{borrowId:int}")]
        public async Task<IActionResult> ExtendedBook(int borrowId,[FromBody]IssueBookDto dto)
        {
            var result = await BorrowService.ExtendBookAsync(FlowSide.Server, borrowId,dto.MemberId);
            return Ok(result);
        }

        [HttpGet("history/member/{memberId:int}")]
        public async Task<ActionResult<ResultResolver<List<BorrowedBookView>>>> GetBorrowHistoryByMember(int memberId)
        {
            var result = await BorrowService.GetBorrowHistoryByMemberIdAsync(FlowSide.Server, memberId);
            return Ok(result);
        }

        [HttpGet("history/book/{bookId:int}")]
        public async Task<ActionResult<ResultResolver<List<BorrowedBookView>>>> GetBorrowHistoryByBook(int bookId)
        {
            var result = await BorrowService.GetBorrowHistoryByBookIdAsync(FlowSide.Server, bookId);
            return Ok(result);
        }

        [HttpGet("history/all")]
        public async Task<ActionResult<ResultResolver<List<BorrowedBookView>>>> GetAllBorrowHistory()
        {
            var result = await BorrowService.GetBorrowHistoryEveryThingAsync(FlowSide.Server);
            return Ok(result);
        }
    }
}