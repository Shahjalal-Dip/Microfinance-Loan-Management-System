using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{
    public class RepaymentService
    {
        private RepaymentRepository repaymentRepository;
        private LoanRepository loanRepository;

        public RepaymentService()
        {
            repaymentRepository = new RepaymentRepository();
            loanRepository = new LoanRepository();
        }

        public bool RecordPayment(Repayment payment)
        {
            try
            {
                if (!ValidatePayment(payment))
                {
                    return false;
                }

                if (payment.PaymentDate > payment.DueDate)
                {
                    payment.LateFee = CalculateLateFee(payment);
                }

                bool result = repaymentRepository.Insert(payment);

                if (result)
                {
                    UpdateLoanSchedule(payment);
                    CheckLoanCompletion(payment.LoanID);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Payment recording failed: " + ex.Message);
            }
        }

        public List<Repayment> GetRepaymentsByLoan(int loanID)
        {
            return repaymentRepository.GetRepaymentsByLoan(loanID);
        }

        public List<Repayment> GetOverdueRepayments()
        {
            return repaymentRepository.GetOverdueRepayments();
        }

        public decimal GetOutstandingBalance(int loanID)
        {
            Loan loan = loanRepository.GetById(loanID);
            if (loan?.ApprovedAmount == null) return 0;

            decimal totalPayable = loan.GetTotalPayableAmount();
            List<Repayment> payments = GetRepaymentsByLoan(loanID);
            decimal totalPaid = payments.Sum(p => p.AmountPaid);

            return totalPayable - totalPaid;
        }

        private bool ValidatePayment(Repayment payment)
        {
            if (payment.AmountPaid <= 0) return false;
            if (payment.LoanID <= 0) return false;

            Loan loan = loanRepository.GetById(payment.LoanID);
            return loan != null && loan.Status == "Approved";
        }

        private decimal CalculateLateFee(Repayment payment)
        {
            Loan loan = loanRepository.GetById(payment.LoanID);
            if (loan == null) return 0;

            LoanPolicyRepository policyRepo = new LoanPolicyRepository();
            LoanPolicy policy = policyRepo.GetById(loan.PolicyID);

            if (policy == null) return 0;

            int daysLate = (payment.PaymentDate - payment.DueDate).Days;
            return payment.AmountDue * (policy.PenaltyRate / 100) * (daysLate / 30m); 
        }

        private void UpdateLoanSchedule(Repayment payment)
        {
            string query = @"UPDATE LoanSchedule SET IsPaid = 1 
                        WHERE LoanID = @LoanID AND InstallmentNumber = @InstallmentNumber";

            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanID", payment.LoanID);
                cmd.Parameters.AddWithValue("@InstallmentNumber", payment.InstallmentNumber);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CheckLoanCompletion(int loanID)
        {
            string query = @"SELECT COUNT(*) FROM LoanSchedule WHERE LoanID = @LoanID AND IsPaid = 0";

            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanID", loanID);

                conn.Open();
                int unpaidInstallments = (int)cmd.ExecuteScalar();

                if (unpaidInstallments == 0)
                {
                    Loan loan = loanRepository.GetById(loanID);
                    if (loan != null)
                    {
                        loan.Status = "Completed";
                        loanRepository.Update(loan);
                    }
                }
            }
        }
    }
}