using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class LoanSchedule
    {
        public int ScheduleID { get; set; }
        public int LoanID { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
