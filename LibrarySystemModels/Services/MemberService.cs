using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Services
{
    public static class MemberService
    {
        private const string MemberServiceUrl = "api/Members/";

        public static async Task<ResultResolver<Member>> AddMemberAsync(FlowSide side, Member member)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to add members! , action was written to report";
                if (side == FlowSide.Client)
                {
                    
                    await ReportingService.ReportEventAsync(FlowSide.Client, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Member>(null!, false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Insert<Member,Member>(MemberServiceUrl, member);

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(member));
                return new ResultResolver<Member>(member, true, "");
            }
            catch (Exception e)
            {
                var info = string.IsNullOrEmpty(member.FullName) ? "memberId : " + member.MemberID : member.FullName;
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, $"User tried to insert new member: {info}, DB error: {e.Message}");
                return new ResultResolver<Member>(null!, false, $"couldn't insert new member: {info}");
            }
        }

        public static async Task<ResultResolver<Member>> UpdateMemberAsync(FlowSide side, Member member)
        {
            var info = string.IsNullOrEmpty(member.FullName) ? "memberId : " + member.MemberID : member.FullName;
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                var errorMessage = $"User try to update {info} without enough permission";
                if (side == FlowSide.Client)
                {
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Update<Member,Member>(MemberServiceUrl, member);

            // Validate on server: member must exist
            var allRes = await GetAllMembersAsync(FlowSide.Server);
            if (!allRes.ActionResult)
                return new ResultResolver<Member>(new Member(), false, allRes.Message);

            var m = allRes.Data.FirstOrDefault(b => b.MemberID == member.MemberID);
            if (m == null)
                return new ResultResolver<Member>(new Member(), false, "Couldn't find member with MemberId");

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Update(member));
                return new ResultResolver<Member>(member, true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed updating member {info} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<Member>> DeleteMemberAsync(FlowSide side, int memberId)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Admin))
            {
                var errorMessage = $"User try to delete member id: {memberId} without enough permission";
                if (side == FlowSide.Client)
                {
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Delete<Member>(MemberServiceUrl + $"{memberId}");

            try
            {
                await Task.Run(() => DataBaseService.GetLocalDatabase().Delete<Member>(memberId));
                return new ResultResolver<Member>(new Member(), true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to delete member {memberId} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<List<Member>>> GetAllMembersAsync(FlowSide side)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to get members! , action was written to report";
                if (side == FlowSide.Client)
                {
                    
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<List<Member>>(new List<Member>(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Get<List<Member>>(MemberServiceUrl);

            try
            {
                var members = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Member>());
                return new ResultResolver<List<Member>>(members, true, "");
            }
            catch (Exception e)
            {
                var errorMessage = $"couldn't get all members because of DB error: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<List<Member>>(new List<Member>(), false, errorMessage);
            }
        }

        public static async Task<ResultResolver<Member>> GetMemberAsync(FlowSide side, int memberId)
        {
            if (!SessionHelperService.IsEnoughPermission(side, UserRole.Librarian))
            {
                const string errorMessage = "You do not have permission to get Member! , action was written to report";
                if (side == FlowSide.Client)
                {
                    
                    await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                }
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }

            if (side == FlowSide.Client)
                return await DataBaseService.Get<Member>(MemberServiceUrl + $"{memberId}");

            try
            {
                var members = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<Member>());
                var member = members.FirstOrDefault(m => m.MemberID == memberId);
                if (member != null)
                    return new ResultResolver<Member>(member, true, "");
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, "Member not found: " + memberId);
                return new ResultResolver<Member>(new Member(), false, $"Error finding member id: {memberId}");
            }
            catch (Exception e)
            {
                var errorMessage = $"couldn't get member {memberId} in database: {e.Message}";
                await ReportingService.ReportEventAsync(side, SeverityLevel.LOW, errorMessage);
                return new ResultResolver<Member>(new Member(), false, errorMessage);
            }
        }
    }
}
