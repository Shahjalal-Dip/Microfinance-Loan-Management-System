using System;
using System.Xml.Linq;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class Member : User
    {
        public string NID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public decimal Income { get; set; }
        public int? GroupID { get; set; }
        public string GroupName { get; set; }
        public DateTime JoinDate { get; set; }
       
        public decimal GetMaxLoanEligibility()
        {
            return Income * 12;
        }
    }

}