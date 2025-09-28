using System.Xml.Linq;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class Admin : User
    {
        public string Department { get; set; }

        public override string GetDisplayInfo()
        {
            return $"Admin: {Name} - Department: {Department}";
        }
    }
}