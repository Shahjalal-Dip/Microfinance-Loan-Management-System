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

    }
}