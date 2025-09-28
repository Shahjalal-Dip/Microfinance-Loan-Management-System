using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Linq;

namespace Microfinance_Loan_Management_System.Utilities
{
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
        public static DateTime LoginTime { get; set; }

        public static void StartSession(User user)
        {
            CurrentUser = user;
            LoginTime = DateTime.Now;
        }

        public static void EndSession()
        {
            CurrentUser = null;
            LoginTime = default(DateTime);
        }

        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        public static bool HasRole(string role)
        {
            return CurrentUser?.Role?.Equals(role, StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool IsAdmin()
        {
            return HasRole("Admin");
        }

        public static bool IsLoanOfficer()
        {
            return HasRole("LoanOfficer");
        }

        public static bool IsMember()
        {
            return HasRole("Member");
        }
    }

}