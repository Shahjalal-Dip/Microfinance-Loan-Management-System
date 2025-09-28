using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microfinance_Loan_Management_System.utilities
{
    public static class AppSettings
    {
        public static string ApplicationName = "Microfinance Loan Management System";
        public static string Version = "1.0.0";
        public static string Company = "FundNest";

        public static string DefaultConnectionString = "Server=localhost;Database=MicrofinanceDB;Integrated Security=true;";

        public static decimal MaxLoanAmount = 1000000m;
        public static int MaxLoanDurationMonths = 60;
        public static int MinLoanDurationMonths = 3;
        public static decimal DefaultInterestRate = 12.0m;
        public static decimal DefaultPenaltyRate = 2.0m;

        public static int SessionTimeoutMinutes = 30;
        public static string DateFormat = "dd/MM/yyyy";
        public static string CurrencyFormat = "৳ #,##0.00";
    }
}
