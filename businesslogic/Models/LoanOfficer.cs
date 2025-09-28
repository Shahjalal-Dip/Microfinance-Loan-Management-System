using System;
using System.Xml.Linq;
namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class LoanOfficer : User
    {
        public int? AssignedGroupID { get; set; }
        public string AssignedGroupName { get; set; }
        public DateTime HireDate { get; set; }

        public override string GetDisplayInfo()
        {
            return $"Loan Officer: {Name} - Group: {AssignedGroupName ?? "Not Assigned"}";
        }
    }
}