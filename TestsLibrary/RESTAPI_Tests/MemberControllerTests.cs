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

public class MemberControllerTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _dbTestPath;
    private readonly string _sessionToken;
    private readonly MembersController _membersController;
    
    public MemberControllerTests(ITestOutputHelper testOutputHelper)
    {
        DataBaseService.InitLocalDb(Path.Combine("Resources", $"test_{Guid.NewGuid()}.sqlite"));
        _membersController = new MembersController();
        Utils.SetFakeUser(_membersController,null);

    }
    

    public void Dispose()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [Fact]
    public async Task AddMember_ReturnsOk_ResultIsSuccess()
    {
        // Arrange
        var memberName = "Test Member";
        var member = new Member()
        {
            FullName = memberName ,
            Email = "Tset@test.com",
            Phone = "sdsda"
        };
        // Act
        var result = await _membersController.AddMember(member);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceResult = Assert.IsType<ResultResolver<Member>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);
        Assert.NotNull(serviceResult.Data);
    }

    [Fact]
    public async Task GetMemberByIdtest()
    {
        var memberName = "Test Member";
        var member = new Member()
        {
            FullName = memberName ,
            Email = "Tset@test.com",
            Phone = "sdsda"
        };
        await _membersController.AddMember(member);

        // Ensure we fetch the actual new BookId from DB
        var allMembers = await _membersController.GetAllMembers();
        var firstResult = Assert.IsType<OkObjectResult>(allMembers.Result);
        var resolverMembers = Assert.IsType<ResultResolver<List<Member>>>(firstResult.Value);
        Assert.True(resolverMembers.ActionResult);
        var addedMember = resolverMembers.Data.FirstOrDefault(m => m.FullName.Equals(memberName));
        Assert.NotNull(addedMember);
        Assert.True(member.Equals(addedMember));
        var result = await _membersController.GetMember(addedMember.MemberID);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var serviceResult = Assert.IsType<ResultResolver<Member>>(okResult.Value);
        Assert.True(serviceResult.ActionResult);
        Assert.Equal(memberName,serviceResult.Data.FullName);
    }

    [Fact]
    public async Task DeleteMemberTest()
    {
        
        var memberName = "Test Member";
        var member = new Member()
        {
            FullName = memberName ,
            Email = "Tset@test.com",
            Phone = "sdsda"
        };
        await _membersController.AddMember(member);

        // Ensure we fetch the actual new BookId from DB
        var allMembers = await _membersController.GetAllMembers();
        var firstResult = Assert.IsType<OkObjectResult>(allMembers.Result);
        var resolverMembers = Assert.IsType<ResultResolver<List<Member>>>(firstResult.Value);
        Assert.True(resolverMembers.ActionResult);
        var addedMember = resolverMembers.Data.FirstOrDefault(m => m.FullName.Equals(memberName));
        Assert.NotNull(addedMember);
        Assert.True(member.Equals(addedMember));
        var result = await _membersController.DeleteMember(addedMember.MemberID);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceResult = Assert.IsType<ResultResolver<Member>>(okResult.Value);       
        Assert.True(serviceResult.ActionResult);
    }

    [Fact]
    public async Task UpdateMember_UpdatesData()
    {
        const string memberName = "Test Member";
        const string memberName2 = "Test2 Member";
        
        var member = new Member()
        {
            FullName = memberName ,
            Email = "Tset@test.com",
            Phone = "sdsda"
        };
        await _membersController.AddMember(member);

        // Ensure we fetch the actual new BookId from DB
        var allMembers = await _membersController.GetAllMembers();
        var firstResult = Assert.IsType<OkObjectResult>(allMembers.Result);
        var resolverMembers = Assert.IsType<ResultResolver<List<Member>>>(firstResult.Value);
        Assert.True(resolverMembers.ActionResult);
        var addedMember = resolverMembers.Data.FirstOrDefault(m => m.FullName.Equals(memberName));
        Assert.NotNull(addedMember);
        Assert.True(member.Equals(addedMember));
        addedMember.FullName = memberName2;
        var result = await _membersController.UpdateMember(addedMember);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceResult = Assert.IsType<ResultResolver<Member>>(okResult.Value);       
        Assert.True(serviceResult.ActionResult);
        Assert.NotEqual(memberName, serviceResult.Data.FullName);
        Assert.Equal(memberName2, serviceResult.Data.FullName);
    }
}