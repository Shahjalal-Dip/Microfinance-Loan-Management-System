using System;
using System.Collections.Generic;

namespace Microfinance_Loan_Management_System.BusinessLogic.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupLeader { get; set; }
        public int MaxMembers { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<Member> Members { get; set; }

        public Group()
        {
            Members = new List<Member>();
        }

        public int GetCurrentMemberCount()
        {
            return Members?.Count ?? 0;
        }
    }
}