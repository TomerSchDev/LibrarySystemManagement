using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Services
{
    public static class ReportingService
    {
        private const string ReportsServiceUrl = "api/Reports/";

        public static async Task<ResultResolver<Report>> AddReportAsync(FlowSide side, Report report)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Insert<Report,Report>(ReportsServiceUrl, report);

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(report));
                return new ResultResolver<Report>(report, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<Report>(null!, false, "DB Error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<Report>> UpdateReportAsync(FlowSide side, Report report)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Update<Report, Report>(ReportsServiceUrl + $"{report.ReportID}", report);

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Update(report));
                return new ResultResolver<Report>(report, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<Report>(null!, false, "DB Error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<Report>> DeleteReportAsync(FlowSide side, int reportId)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Delete<Report>(ReportsServiceUrl + $"{reportId}");

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Delete<Report>(reportId));
                return new ResultResolver<Report>(null!, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<Report>(null!, false, "DB Error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<List<Report>>> GetReportsAsync(FlowSide side)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<Report>>(ReportsServiceUrl);

            try
            {
                var items = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Report>() );
                return new ResultResolver<List<Report>>(items, true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<List<Report>>([], false, "DB Error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<List<Report>>> GetReportsByUserAsync(FlowSide side, User user)
        {
            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<Report>>(ReportsServiceUrl + $"user/{user.UserID}");

            try
            {
                var all = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Report>());
                return new ResultResolver<List<Report>>(all.Where(r => r.UserId == user.UserID).ToList(), true, "");
            }
            catch (Exception ex)
            {
                return new ResultResolver<List<Report>>([], false, "DB Error: " + ex.Message);
            }
        }

        // Async reporting event utility
        public static async Task ReportEventAsync(FlowSide side, SeverityLevel severityLevel, string message)
        {
            var user =  await SessionHelperService.GetCurrentUser(side);

            var report = new Report(severityLevel, user.Username, user.UserID, user.Role, DateTime.Now, message);

            if (side == FlowSide.Client)
            {
                await DataBaseService.Insert<Report,Report>(ReportsServiceUrl, report);
                return;
            }

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(report));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DB when inserting report, error: " + ex.Message);
            }
        }

        public static async Task<ResultResolver<List<Report>>> GetReportsWithPermissionAsync(FlowSide side)
        {
            if (side == FlowSide.Client)
            {
                return await DataBaseService.Get<List<Report>>(ReportsServiceUrl+"permission");
            }
            var currentUser = await SessionHelperService.GetCurrentUser(side);
            try
            {
                var reports = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Report>());
                var reportsAllowed = reports.Where(r => r.Role <= currentUser.Role);
                return new ResultResolver<List<Report>>(reportsAllowed.ToList(), true, "");
            }
            catch (Exception ex)
            {
                var errorMessage = "Error in DB when inserting reports, error: " + ex.Message;
                await ReportEventAsync(side, SeverityLevel.HIGH, errorMessage);
                return new ResultResolver<List<Report>>([], false, "DB Error: " + errorMessage);
            }
        }
    }
}
