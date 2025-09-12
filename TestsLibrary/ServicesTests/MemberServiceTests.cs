using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using Xunit;

namespace TestsLibrary
{
    public class MemberServiceTests : IDisposable
    {
        private readonly string _dbTestPath;

        public MemberServiceTests()
        {
            _dbTestPath = Path.Combine("Resources", $"test_member_{Guid.NewGuid()}.sqlite");
            DataBaseService.SetModes(false, true);
            DataBaseService.Init(_dbTestPath);
            Utils.LogInUser(null, FlowSide.Client);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            // if (File.Exists(_dbTestPath)) File.Delete(_dbTestPath);
        }

        [Fact]
        public async Task CanAddAndGetMember()
        {
            var member = new Member { FullName = "Test Member",Email = "sdsd",Phone = "sad"};
            var addResult = await MemberService.AddMemberAsync(FlowSide.Client, member);
            Assert.True(addResult.ActionResult);
            var membersRes = await MemberService.GetAllMembersAsync(FlowSide.Client);
            Assert.True(membersRes.ActionResult);
            var foundMember = membersRes.Data.FirstOrDefault(x => x.Equals(member));
            Assert.NotNull(foundMember);
            var getResult = await MemberService.GetMemberAsync(FlowSide.Client, foundMember.MemberID);
            Assert.True(getResult.ActionResult);
            Assert.Equal("Test Member", getResult.Data.FullName);
        }

        [Fact]
        public async Task CanUpdateMember()
        {
            var member = new Member { FullName = "Old Name", MemberID = 201 };
            await MemberService.AddMemberAsync(FlowSide.Client, member);
            
            var membersRes = await MemberService.GetAllMembersAsync(FlowSide.Client);
            Assert.True(membersRes.ActionResult);
            var foundMember = membersRes.Data.FirstOrDefault(x => x.Equals(member));
            Assert.NotNull(foundMember);
            foundMember.FullName = "Updated Name";
            var updateResult = await MemberService.UpdateMemberAsync(FlowSide.Client, foundMember);
            Assert.True(updateResult.ActionResult);
            var getResult = await MemberService.GetMemberAsync(FlowSide.Client, foundMember.MemberID);
            Assert.Equal("Updated Name", getResult.Data.FullName);
        }

        [Fact]
        public async Task CanDeleteMember()
        {
            var member = new Member { FullName = "Del Member", MemberID = 202 };
            await MemberService.AddMemberAsync(FlowSide.Client, member);
            var delResult = await MemberService.DeleteMemberAsync(FlowSide.Client, member.MemberID);
            Assert.True(delResult.ActionResult);
            var getResult = await MemberService.GetMemberAsync(FlowSide.Client, member.MemberID);
            Assert.False(getResult.ActionResult);
        }
    }
}
