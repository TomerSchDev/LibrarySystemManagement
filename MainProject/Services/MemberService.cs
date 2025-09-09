using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Microsoft.Data.Sqlite;

namespace Library_System_Management.Services
{
    public static class MemberService
    {
        public static void AddMember(Member member)
        {
            if (!SessionHelperService.IsEnoughPermission(UserRole.Librarian))
            {
                MessageBox.Show("couldn't add new Member : Not enough Permission" , "Warnning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                DatabaseManager.Insert(member);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't insert member to DatabaseManager with expectation : "+e.Message);
            }
        }

        public static List<Member> GetAllMembers()
        {
            List<Member> members;
            try
            {
                members = DatabaseManager.SelectAll<Member>();
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't select all members from  DatabaseManager with expectation : "+e.Message);
                return [];
            }

            return members;
        }

        public static void UpdateMember(Member member)
        {
            if (!SessionHelperService.IsEnoughPermission(UserRole.Librarian))
            {
                MessageBox.Show("couldn't update Member : Not enough Permission" , "Warnning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                DatabaseManager.Update(member);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't update member from  DatabaseManager with expectation : "+e.Message);
                
            }
        }

        public static void DeleteMember(int memberId)
        {
            if (!SessionHelperService.IsEnoughPermission(UserRole.Admin))
            {
                MessageBox.Show("couldn't remove Member : Not enough Permission" , "Warnning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
               DatabaseManager.Delete<Member>(memberId);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't select all members from  DatabaseManager with expectation : "+e.Message);
            }
        }

        public static Member? GetMember(int memberId)
        {
            return GetAllMembers().Find(member => member.MemberID == memberId);
        }
    }
}
