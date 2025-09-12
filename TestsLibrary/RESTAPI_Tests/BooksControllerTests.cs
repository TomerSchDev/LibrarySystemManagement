using System.Security.Claims;
using LibraryRestApi.Controllers;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace TestsLibrary.RESTAPI_Tests;

public class BooksControllerTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _dbTestPath;
    private readonly string _sessionToken;
    private readonly BooksController _controller;
    public BooksControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _dbTestPath = Path.Combine("Resources", $"test_{Guid.NewGuid()}.sqlite");
        DataBaseService.SetModes(true, false);
        DataBaseService.Init(_dbTestPath);
        _controller = new BooksController();
        Utils.SetFakeUser(_controller,null);

    }
    

    public void Dispose()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [Fact]
    public async Task AddBook_ReturnsOk_ResultIsSuccess()
    {
        // Arrange
      
        var book = new Book
        {
            Title = "Integration Test Book",
            Author = "Jane Doe",
            ISBN = "9876543210"
        };

        // Act
        var result = await _controller.AddBook(book);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceResult = Assert.IsType<ResultResolver<Book>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);
        Assert.NotNull(serviceResult.Data);
    }

    [Fact]
    public async Task GetBookById_ReturnsOk_AfterAdding()
    {
        var book = new Book { Title = "Find Me", Author = "A", ISBN = "1111" };
        await _controller.AddBook(book);

        // Ensure we fetch the actual new BookId from DB
        var allBooks = await BookService.GetAllBooksAsync(FlowSide.Server);
        var addedBook = allBooks.Data.First(b => b.Title == "Find Me");

        var result = await _controller.GetBookById(addedBook.BookID);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var serviceResult = Assert.IsType<ResultResolver<Book>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);
        Assert.Equal("Find Me", serviceResult.Data.Title);
    }

    [Fact]
    public async Task DeleteBook_RemovesBook()
    {
        var book = new Book { Title = "Delete Me", Author = "B", ISBN = "2222" };
        await _controller.AddBook(book);

        var allBooks = await BookService.GetAllBooksAsync(FlowSide.Server);
        var addedBook = allBooks.Data.First(b => b.Title == "Delete Me");

        var deleteResult = await _controller.DeleteBook(addedBook.BookID);
        var okResult = Assert.IsType<OkObjectResult>(deleteResult);
        var serviceResult = Assert.IsType<ResultResolver<Book>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);

        var getResult = await _controller.GetBookById(addedBook.BookID);
        var okResult2 = Assert.IsType<OkObjectResult>(getResult.Result);
        var getResolver = Assert.IsType<ResultResolver<Book>>(okResult2.Value);
        Assert.False(getResolver.ActionResult);
    }

    [Fact]
    public async Task UpdateBook_UpdatesData()
    {
        var book = new Book { Title = "Update Me", Author = "C", ISBN = "3333" };
        await _controller.AddBook(book);

        var allBooks = await BookService.GetAllBooksAsync(FlowSide.Server);
        var addedBook = allBooks.Data.First(b => b.Title == "Update Me");

        addedBook.Title = "Updated Title";
        addedBook.Author = "Updated Author";

        var updateResult = await _controller.UpdateBook(addedBook);
        var okResult = Assert.IsType<OkObjectResult>(updateResult);
        var serviceResult = Assert.IsType<ResultResolver<Book>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);

        var getResult = await _controller.GetBookById(addedBook.BookID);
        var okResult2 = Assert.IsType<OkObjectResult>(getResult.Result);
        var getResolver = Assert.IsType<ResultResolver<Book>>(okResult2.Value);

        Assert.Equal("Updated Title", getResolver.Data.Title);
        Assert.Equal("Updated Author", getResolver.Data.Author);
    }
}