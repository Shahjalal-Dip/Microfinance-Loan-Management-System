using System;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    
    public class LoanPolicy
    {
        public int PolicyID { get; set; }
        public string LoanType { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal PenaltyRate { get; set; }
        public int MinDurationMonths { get; set; }
        public int MaxDurationMonths { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public decimal CalculateEMI(decimal principal, int durationMonths)
        {
            decimal monthlyRate = (InterestRate / 100) / 12;
            if (monthlyRate == 0) return principal / durationMonths;

            double factor = Math.Pow((double)(1 + monthlyRate), durationMonths);
            decimal emi = principal * monthlyRate * (decimal)factor / ((decimal)factor - 1);
            return Math.Round(emi, 2);
        }
    }
}