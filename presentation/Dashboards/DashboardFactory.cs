using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.Presentation.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Dashboards
{
    public static class DashboardFactory
    {
        public static Form CreateDashboard(User user)
        {
            switch(user.Role.ToLower())
            {
                case "admin":
                    return new AdminDashboard();
                case "loanofficer":
                    return new OfficerDashboard();
                case "member":
                    return new MemberDashboard();
                default:
                    MessageBox.Show("Unknown user role. Cannot determine dashboard.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
            }
        }
    }
}
