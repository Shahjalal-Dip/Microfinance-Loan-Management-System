using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{

    public class LoanService
    {
        private LoanRepository loanRepository;
        private LoanPolicyRepository policyRepository;
        private MemberRepository memberRepository;

        public LoanService()
        {
            loanRepository = new LoanRepository();
            policyRepository = new LoanPolicyRepository();
            memberRepository = new MemberRepository();
        }

        public bool ApplyForLoan(Loan loan)
        {
            try
            {
                if (!ValidateLoanApplication(loan))
                {
                    return false;
                }

                loan.Status = "Pending";
                loan.ApplicationDate = DateTime.Now;

                return loanRepository.Insert(loan);
            }
            catch (Exception ex)
            {
                throw new Exception("Loan application failed: " + ex.Message);
            }
        }

        public bool ApproveLoan(int loanID, decimal approvedAmount, decimal interestRate, int officerID)
        {
            try
            {
                Loan loan = loanRepository.GetById(loanID);
                if (loan == null || loan.Status != "Pending")
                {
                    return false;
                }

                loan.ApprovedAmount = approvedAmount;
                loan.InterestRate = interestRate;
                loan.Status = "Approved";
                loan.ApprovalDate = DateTime.Now;
                loan.OfficerID = officerID;

                bool result = loanRepository.Update(loan);

                if (result)
                {
                    GenerateLoanSchedule(loan);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Loan approval failed: " + ex.Message);
            }
        }

        public bool RejectLoan(int loanID, int officerID)
        {
            try
            {
                Loan loan = loanRepository.GetById(loanID);
                if (loan == null || loan.Status != "Pending")
                {
                    return false;
                }

                loan.Status = "Rejected";
                loan.ApprovalDate = DateTime.Now;
                loan.OfficerID = officerID;

                return loanRepository.Update(loan);
            }
            catch (Exception ex)
            {
                throw new Exception("Loan rejection failed: " + ex.Message);
            }
        }

        public List<Loan> GetLoansByMember(int memberID)
        {
            return loanRepository.GetLoansByMember(memberID);
        }

        public List<Loan> GetPendingLoans()
        {
            return loanRepository.GetPendingLoans();
        }

        public decimal CalculateEMI(decimal principal, decimal interestRate, int durationMonths)
        {
            decimal monthlyRate = (interestRate / 100) / 12;
            if (monthlyRate == 0) return principal / durationMonths;

            double factor = Math.Pow((double)(1 + monthlyRate), durationMonths);
            decimal emi = principal * monthlyRate * (decimal)factor / ((decimal)factor - 1);
            return Math.Round(emi, 2);
        }

        private bool ValidateLoanApplication(Loan loan)
        {
            Member member = memberRepository.GetById(loan.MemberID);
            if (member == null) return false;

            LoanPolicy policy = policyRepository.GetById(loan.PolicyID);
            if (policy == null || !policy.IsActive) return false;

            if (loan.AmountRequested > policy.MaxAmount) return false;

            if (loan.DurationMonths < policy.MinDurationMonths ||
                loan.DurationMonths > policy.MaxDurationMonths) return false;

            if (loan.AmountRequested > member.GetMaxLoanEligibility()) return false;

            return true;
        }

        public DataTable LoadLoanScheduleByUser(int userId)
        {
            string query = @"SELECT ls.LoanID,ls.DueDate,ls.TotalAmount AS AmountDue,l.Status
                             FROM LoanSchedule ls
                             JOIN Loans l ON ls.LoanID = l.LoanID
                             WHERE l.MemberID = @UserID AND IsPaid = 0";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConnection.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

        private void GenerateLoanSchedule(Loan loan)
        {
            if (!loan.ApprovedAmount.HasValue) return;

            decimal monthlyEMI = CalculateEMI(loan.ApprovedAmount.Value, loan.InterestRate, loan.DurationMonths);
            decimal principal = loan.ApprovedAmount.Value;
            decimal monthlyInterestRate = (loan.InterestRate / 100) / 12;

            DateTime currentDueDate = loan.ApprovalDate?.AddMonths(1) ?? DateTime.Now.AddMonths(1);

            for (int i = 1; i <= loan.DurationMonths; i++)
            {
                decimal interestAmount = principal * monthlyInterestRate;
                decimal principalAmount = monthlyEMI - interestAmount;

                if (i == loan.DurationMonths)
                {
                    principalAmount = principal;
                    monthlyEMI = principalAmount + interestAmount;
                }

                LoanSchedule schedule = new LoanSchedule
                {
                    LoanID = loan.LoanID,
                    InstallmentNumber = i,
                    DueDate = currentDueDate,
                    PrincipalAmount = Math.Round(principalAmount, 2),
                    InterestAmount = Math.Round(interestAmount, 2),
                    TotalAmount = Math.Round(monthlyEMI, 2),
                    IsPaid = false
                };
                InsertLoanSchedule(schedule);

                principal -= principalAmount;
                currentDueDate = currentDueDate.AddMonths(1);
            }
        }

        private void InsertLoanSchedule(LoanSchedule schedule)
        {
            string query = @"INSERT INTO LoanSchedule (LoanID, InstallmentNumber, DueDate, PrincipalAmount, 
                        InterestAmount, TotalAmount, IsPaid) 
                        VALUES (@LoanID, @InstallmentNumber, @DueDate, @PrincipalAmount, 
                        @InterestAmount, @TotalAmount, @IsPaid)";

            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanID", schedule.LoanID);
                cmd.Parameters.AddWithValue("@InstallmentNumber", schedule.InstallmentNumber);
                cmd.Parameters.AddWithValue("@DueDate", schedule.DueDate);
                cmd.Parameters.AddWithValue("@PrincipalAmount", schedule.PrincipalAmount);
                cmd.Parameters.AddWithValue("@InterestAmount", schedule.InterestAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", schedule.TotalAmount);
                cmd.Parameters.AddWithValue("@IsPaid", schedule.IsPaid);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public decimal GetTodayCollections()
        {
            string query = @"SELECT SUM(AmountPaid) FROM Repayments 
                         WHERE CAST(PaymentDate AS DATE) = CAST(GETDATE() AS DATE)";
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        public DataTable TodayCollection()
        {
            string query = @" SELECT r.PaymentID, r.LoanID, r.AmountPaid, r.PaymentDate, u.Name AS MemberName
                              FROM Repayments r
                              INNER JOIN Loans l ON r.LoanID = l.LoanID
                              INNER JOIN Members m ON l.MemberID = m.MemberID
                              INNER JOIN Users u ON m.MemberID = u.UserID
                              WHERE CAST(r.PaymentDate AS DATE) = CAST(GETDATE() AS DATE)";

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(DatabaseConnection.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }
            return dataTable;
        }


        public int GetActiveLoansCount()
        {
            string query = @"SELECT COUNT(*) FROM Loans WHERE Status IN ('Approved', 'Disbursed')";
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        
    }
}
