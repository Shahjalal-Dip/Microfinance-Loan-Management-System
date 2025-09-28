using System;
namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{

    public class Repayment
    {
        public int PaymentID { get; set; }
        public int LoanID { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LateFee { get; set; }
        public string PaymentMethod { get; set; }
        public int? CollectedBy { get; set; }
        public string Notes { get; set; }

        public bool IsOverdue()
        {
            return DateTime.Now > DueDate && AmountPaid < AmountDue;
        }

        public int GetDaysOverdue()
        {
            if (!IsOverdue()) return 0;
            return (DateTime.Now - DueDate).Days;
        }
    }
}