using System;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
  
    public abstract class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public abstract string GetDisplayInfo();
    }
}