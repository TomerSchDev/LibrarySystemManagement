using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using LibrarySystemModels.Helpers;
using Xunit;

namespace TestsLibrary
{
    public class ReportingServiceTests : IDisposable
    {
        private readonly string _dbTestPath;

        public ReportingServiceTests()
        {
            _dbTestPath = Path.Combine("Resources", $"test_report_{Guid.NewGuid()}.sqlite");
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
        public async Task CanAddAndGetReport()
        {
           
            await ReportingService.ReportEventAsync(FlowSide.Client,SeverityLevel.LOW,"Test report");
            var getResult = await ReportingService.GetReportsAsync(FlowSide.Client);
            Assert.True(getResult.ActionResult);
            Assert.Contains(getResult.Data, r => r.ReportMessage == "Test report");
        }

        [Fact]
        public async Task CanDeleteReport()
        {
            const string reportMessage = "For delete";
           
            await ReportingService.ReportEventAsync(FlowSide.Client,SeverityLevel.LOW,reportMessage);
            var allReports = await ReportingService.GetReportsAsync(FlowSide.Client);
            var rep = allReports.Data.First(r => r.ReportMessage == reportMessage);
            var delResult = await ReportingService.DeleteReportAsync(FlowSide.Client, rep.ReportID);
            Assert.True(delResult.ActionResult);
        }
    }
}
