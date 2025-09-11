using LibrarySystemModels.Models;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryRestApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Report report)
        {
            var result = await ReportingService.AddReportAsync(FlowSide.Server, report);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Report report)
        {
            if (report.ReportID == 0) report.ReportID = id;
            var result = await ReportingService.UpdateReportAsync(FlowSide.Server, report);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await ReportingService.DeleteReportAsync(FlowSide.Server, id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ResultResolver<List<Report>>>> GetAll()
        {
            var result = await ReportingService.GetReportsAsync(FlowSide.Server);
            return Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<ResultResolver<List<Report>>>> GetByUser(int userId)
        {
            var user = await AuthService.GetUserByIdAsync(FlowSide.Server, userId);
            if (LibrarySystemModels.Models.User.IsDefaultUser(user)) return NotFound();
            var result = await ReportingService.GetReportsByUserAsync(FlowSide.Server, user);
            return Ok(result);
        }
        [HttpGet("permission")]
        public async Task<ActionResult<ResultResolver<List<Report>>>> GetByUser()
        {
            var result = await ReportingService.GetReportsWithPermissionAsync(FlowSide.Server);
            return Ok(result);
        }
    }
}