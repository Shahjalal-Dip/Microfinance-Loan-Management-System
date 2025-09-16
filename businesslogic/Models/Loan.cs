using System;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class Loan
    {
        public int LoanID { get; set; }
        public decimal AmountRequested { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int DurationMonths { get; set; }
        public string Status { get; set; }
        public string LoanPurpose { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DisbursementDate { get; set; }
        public int MemberID { get; set; }
        public int? OfficerID { get; set; }
        public int PolicyID { get; set; }
        public int? GroupID { get; set; }
        public string MemberName { get; set; }
        public string OfficerName { get; set; }
        public string PolicyType { get; set; }
   
        public decimal GetTotalPayableAmount()
        {
            if (!ApprovedAmount.HasValue) return 0;

            decimal principal = ApprovedAmount.Value;
            decimal monthlyRate = (InterestRate / 100) / 12;
            decimal totalInterest = principal * monthlyRate * DurationMonths;
            return principal + totalInterest;
        }

        public decimal GetMonthlyEMI()
        {
            if (!ApprovedAmount.HasValue) return 0;

            LoanPolicy policy = new LoanPolicy { InterestRate = InterestRate };
            return policy.CalculateEMI(ApprovedAmount.Value, DurationMonths);
        }
    }
}