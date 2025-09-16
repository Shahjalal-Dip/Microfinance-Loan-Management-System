using Microfinance_Loan_Management_System.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{
    public class ReportService
    {
        public DataTable GetLoanSummaryReport()
        {
            string query = @"SELECT l.Status,COUNT(*) as LoanCount,SUM(l.AmountRequested) as TotalRequested,SUM(l.ApprovedAmount) as TotalApproved
                             FROM Loans l
                             GROUP BY l.Status";

            return ExecuteQuery(query);
        }

        public DataTable GetOverdueLoansReport()
        {
            string query = @"SELECT u.Name AS MemberName,l.LoanID,l.ApprovedAmount,ls.DueDate,ls.TotalAmount AS AmountDue,DATEDIFF(DAY, ls.DueDate, GETDATE()) AS DaysOverdue
                               FROM Loans l
                               INNER JOIN Members m ON l.MemberID = m.MemberID
                               INNER JOIN Users u ON m.MemberID = u.UserID
                               INNER JOIN LoanSchedule ls ON l.LoanID = ls.LoanID
                               WHERE ls.DueDate < GETDATE()
                               AND ls.IsPaid = 0
                               ORDER BY ls.DueDate;";
            return ExecuteQuery(query);
        }

        public DataTable GetCollectionReport(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT u.Name AS MemberName,l.LoanID,r.PaymentDate,r.AmountPaid,r.LateFee,uo.Name AS CollectedBy
                           FROM Repayments r
                           INNER JOIN Loans l ON r.LoanID = l.LoanID
                           INNER JOIN Members m ON l.MemberID = m.MemberID
                           INNER JOIN Users u ON m.MemberID = u.UserID
                           LEFT JOIN LoanOfficers lo ON r.CollectedBy = lo.OfficerID
                           LEFT JOIN Users uo ON lo.OfficerID = uo.UserID
                           WHERE r.PaymentDate BETWEEN @FromDate AND @ToDate
                           ORDER BY r.PaymentDate DESC;";

            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public DataTable GetProfitLossReport(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT 'Interest Income' as Category,SUM(r.AmountPaid - ls.PrincipalAmount) as Amount
                        FROM Repayments r
                        INNER JOIN LoanSchedule ls ON r.LoanID = ls.LoanID AND r.InstallmentNumber = ls.InstallmentNumber
                        WHERE r.PaymentDate BETWEEN @FromDate AND @ToDate
                        UNION ALL
                        SELECT 
                        'Late Fees' as Category,
                        SUM(r.LateFee) as Amount
                        FROM Repayments r
                        WHERE r.PaymentDate BETWEEN @FromDate AND @ToDate AND r.LateFee > 0";

            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private DataTable ExecuteQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}