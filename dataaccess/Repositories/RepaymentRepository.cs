using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.DataAccess.Repositories
{
    public class RepaymentRepository : BaseRepository<Repayment>
    {
        public override List<Repayment> GetAll()
        {
            List<Repayment> repayments = new List<Repayment>();
            string query = @"SELECT PaymentID, LoanID, InstallmentNumber, AmountDue, AmountPaid, PaymentDate, 
                        DueDate, LateFee, PaymentMethod, CollectedBy, Notes FROM Repayments";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Repayment repayment = new Repayment();
                    PopulateRepaymentData(repayment, reader);
                    repayments.Add(repayment);
                }
            }
            return repayments;
        }

        public override Repayment GetById(int id)
        {
            string query = @"SELECT PaymentID, LoanID, InstallmentNumber, AmountDue, AmountPaid, PaymentDate, 
                        DueDate, LateFee, PaymentMethod, CollectedBy, Notes FROM Repayments WHERE PaymentID = @PaymentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Repayment repayment = new Repayment();
                    PopulateRepaymentData(repayment, reader);
                    return repayment;
                }
            }
            return null;
        }

        public List<Repayment> GetRepaymentsByLoan(int loanID)
        {
            List<Repayment> repayments = new List<Repayment>();
            string query = @"SELECT PaymentID, LoanID, InstallmentNumber, AmountDue, AmountPaid, PaymentDate, 
                        DueDate, LateFee, PaymentMethod, CollectedBy, Notes FROM Repayments 
                        WHERE LoanID = @LoanID ORDER BY InstallmentNumber";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanID", loanID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Repayment repayment = new Repayment();
                    PopulateRepaymentData(repayment, reader);
                    repayments.Add(repayment);
                }
            }
            return repayments;
        }

        public List<Repayment> GetOverdueRepayments()
        {
            List<Repayment> repayments = new List<Repayment>();
            string query = @"SELECT PaymentID, LoanID, InstallmentNumber, AmountDue, AmountPaid, PaymentDate, 
                        DueDate, LateFee, PaymentMethod, CollectedBy, Notes FROM Repayments 
                        WHERE DueDate < GETDATE() AND AmountPaid < AmountDue";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Repayment repayment = new Repayment();
                    PopulateRepaymentData(repayment, reader);
                    repayments.Add(repayment);
                }
            }
            return repayments;
        }

        public override bool Insert(Repayment entity)
        {
            string query = @"INSERT INTO Repayments (LoanID, InstallmentNumber, AmountDue, AmountPaid, PaymentDate, 
                        DueDate, LateFee, PaymentMethod, CollectedBy, Notes) 
                        VALUES (@LoanID, @InstallmentNumber, @AmountDue, @AmountPaid, @PaymentDate, 
                        @DueDate, @LateFee, @PaymentMethod, @CollectedBy, @Notes);
                        SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanID", entity.LoanID);
                cmd.Parameters.AddWithValue("@InstallmentNumber", entity.InstallmentNumber);
                cmd.Parameters.AddWithValue("@AmountDue", entity.AmountDue);
                cmd.Parameters.AddWithValue("@AmountPaid", entity.AmountPaid);
                cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
                cmd.Parameters.AddWithValue("@DueDate", entity.DueDate);
                cmd.Parameters.AddWithValue("@LateFee", entity.LateFee);
                cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod ?? "Cash");
                cmd.Parameters.AddWithValue("@CollectedBy", entity.CollectedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", entity.Notes ?? (object)DBNull.Value);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    entity.PaymentID = Convert.ToInt32(result);
                    return true;
                }
            }
            return false;
        }

        public override bool Update(Repayment entity)
        {
            string query = @"UPDATE Repayments SET LoanID = @LoanID, InstallmentNumber = @InstallmentNumber, 
                        AmountDue = @AmountDue, AmountPaid = @AmountPaid, PaymentDate = @PaymentDate, 
                        DueDate = @DueDate, LateFee = @LateFee, PaymentMethod = @PaymentMethod, 
                        CollectedBy = @CollectedBy, Notes = @Notes WHERE PaymentID = @PaymentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentID", entity.PaymentID);
                cmd.Parameters.AddWithValue("@LoanID", entity.LoanID);
                cmd.Parameters.AddWithValue("@InstallmentNumber", entity.InstallmentNumber);
                cmd.Parameters.AddWithValue("@AmountDue", entity.AmountDue);
                cmd.Parameters.AddWithValue("@AmountPaid", entity.AmountPaid);
                cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
                cmd.Parameters.AddWithValue("@DueDate", entity.DueDate);
                cmd.Parameters.AddWithValue("@LateFee", entity.LateFee);
                cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod ?? "Cash");
                cmd.Parameters.AddWithValue("@CollectedBy", entity.CollectedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", entity.Notes ?? (object)DBNull.Value);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public override bool Delete(int id)
        {
            string query = "DELETE FROM Repayments WHERE PaymentID = @PaymentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentID", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        private void PopulateRepaymentData(Repayment repayment, SqlDataReader reader)
        {
            repayment.PaymentID = Convert.ToInt32(reader["PaymentID"]);
            repayment.LoanID = Convert.ToInt32(reader["LoanID"]);
            repayment.InstallmentNumber = Convert.ToInt32(reader["InstallmentNumber"]);
            repayment.AmountDue = Convert.ToDecimal(reader["AmountDue"]);
            repayment.AmountPaid = Convert.ToDecimal(reader["AmountPaid"]);
            repayment.PaymentDate = Convert.ToDateTime(reader["PaymentDate"]);
            repayment.DueDate = Convert.ToDateTime(reader["DueDate"]);
            repayment.LateFee = Convert.ToDecimal(reader["LateFee"]);
            repayment.PaymentMethod = reader["PaymentMethod"].ToString();
            repayment.CollectedBy = reader["CollectedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CollectedBy"]);
            repayment.Notes = reader["Notes"].ToString();
        }
    }
}