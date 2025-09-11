using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryRestApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var result = await BookService.AddBookAsync(FlowSide.Server, book);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            var result = await BookService.UpdateBookAsync(FlowSide.Server, book);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await BookService.DeleteBookAsync(FlowSide.Server, id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ResultResolver<List<Book>>>> GetAllBooks()
        {
            var result = await BookService.GetAllBooksAsync(FlowSide.Server);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResultResolver<Book>>> GetBookById(int id)
        {
            var result = await BookService.GetBookByIdAsync(FlowSide.Server, id);
            return Ok(result);
        }
    }
}