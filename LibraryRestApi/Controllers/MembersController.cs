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
    public class MembersController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] Member member)
        {
            var result = await MemberService.AddMemberAsync(FlowSide.Server, member);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMember([FromBody] Member member)
        {
            var result = await MemberService.UpdateMemberAsync(FlowSide.Server, member);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var result = await MemberService.DeleteMemberAsync(FlowSide.Server, id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ResultResolver<List<Member>>>> GetAllMembers()
        {
            var result = await MemberService.GetAllMembersAsync(FlowSide.Server);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResultResolver<Member>>> GetMember(int id)
        {
            var result = await MemberService.GetMemberAsync(FlowSide.Server, id);
            return Ok(result);
        }
    }
}