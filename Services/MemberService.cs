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
            try
            {
                DatabaseManager.Insert(member);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't insert member to DatabaseManager with expectation : "+e.Message);
                return ;
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
            try
            {
               DatabaseManager.Delete<Member>(memberId);
            }
            catch (Exception e)
            {
                MessageBox.Show("couldn't select all members from  DatabaseManager with expectation : "+e.Message);
            }
        }
    }
}
