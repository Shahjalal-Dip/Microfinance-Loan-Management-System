using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class LoanRepository : BaseRepository<Loan>
{
    public override List<Loan> GetAll()
    {
        List<Loan> loans = new List<Loan>();
        string query = @"SELECT l.LoanID, l.AmountRequested, l.ApprovedAmount, l.InterestRate, l.DurationMonths,
                        l.Status, l.LoanPurpose, l.ApplicationDate, l.ApprovalDate, l.DisbursementDate,
                        l.MemberID, l.OfficerID, l.PolicyID, l.GroupID,
                        u.Name as MemberName, uo.Name as OfficerName, p.LoanType as PolicyType
                        FROM Loans l
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN LoanOfficers lo ON l.OfficerID = lo.OfficerID
                        LEFT JOIN Users uo ON lo.OfficerID = uo.UserID
                        INNER JOIN LoanPolicies p ON l.PolicyID = p.PolicyID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Loan loan = new Loan();
                PopulateLoanData(loan, reader);
                loans.Add(loan);
            }
        }
        return loans;
    }

    public override Loan GetById(int id)
    {
        string query = @"SELECT l.LoanID, l.AmountRequested, l.ApprovedAmount, l.InterestRate, l.DurationMonths,
                        l.Status, l.LoanPurpose, l.ApplicationDate, l.ApprovalDate, l.DisbursementDate,
                        l.MemberID, l.OfficerID, l.PolicyID, l.GroupID,
                        u.Name as MemberName, uo.Name as OfficerName, p.LoanType as PolicyType
                        FROM Loans l
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN LoanOfficers lo ON l.OfficerID = lo.OfficerID
                        LEFT JOIN Users uo ON lo.OfficerID = uo.UserID
                        INNER JOIN LoanPolicies p ON l.PolicyID = p.PolicyID
                        WHERE l.LoanID = @LoanID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LoanID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Loan loan = new Loan();
                PopulateLoanData(loan, reader);
                return loan;
            }
        }
        return null;
    }

    public List<Loan> GetLoansByMember(int memberID)
    {
        List<Loan> loans = new List<Loan>();
        string query = @"SELECT l.LoanID, l.AmountRequested, l.ApprovedAmount, l.InterestRate, l.DurationMonths,
                        l.Status, l.LoanPurpose, l.ApplicationDate, l.ApprovalDate, l.DisbursementDate,
                        l.MemberID, l.OfficerID, l.PolicyID, l.GroupID,
                        u.Name as MemberName, uo.Name as OfficerName, p.LoanType as PolicyType
                        FROM Loans l
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN LoanOfficers lo ON l.OfficerID = lo.OfficerID
                        LEFT JOIN Users uo ON lo.OfficerID = uo.UserID
                        INNER JOIN LoanPolicies p ON l.PolicyID = p.PolicyID
                        WHERE l.MemberID = @MemberID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MemberID", memberID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Loan loan = new Loan();
                PopulateLoanData(loan, reader);
                loans.Add(loan);
            }
        }
        return loans;
    }

    public List<Loan> GetPendingLoans()
    {
        List<Loan> loans = new List<Loan>();
        string query = @"SELECT l.LoanID, l.AmountRequested, l.ApprovedAmount, l.InterestRate, l.DurationMonths,
                        l.Status, l.LoanPurpose, l.ApplicationDate, l.ApprovalDate, l.DisbursementDate,
                        l.MemberID, l.OfficerID, l.PolicyID, l.GroupID,
                        u.Name as MemberName, uo.Name as OfficerName, p.LoanType as PolicyType
                        FROM Loans l
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN LoanOfficers lo ON l.OfficerID = lo.OfficerID
                        LEFT JOIN Users uo ON lo.OfficerID = uo.UserID
                        INNER JOIN LoanPolicies p ON l.PolicyID = p.PolicyID
                        WHERE l.Status = 'Pending'
                        ORDER BY l.ApplicationDate";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Loan loan = new Loan();
                PopulateLoanData(loan, reader);
                loans.Add(loan);
            }
        }
        return loans;
    }

    public override bool Insert(Loan entity)
    {
        string query = @"INSERT INTO Loans (AmountRequested, InterestRate, DurationMonths, Status, 
                        LoanPurpose, ApplicationDate, MemberID, PolicyID, GroupID) 
                        VALUES (@AmountRequested, @InterestRate, @DurationMonths, @Status, 
                        @LoanPurpose, @ApplicationDate, @MemberID, @PolicyID, @GroupID);
                        SELECT SCOPE_IDENTITY();";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@AmountRequested", entity.AmountRequested);
            cmd.Parameters.AddWithValue("@InterestRate", 0); // Will be set during approval
            cmd.Parameters.AddWithValue("@DurationMonths", entity.DurationMonths);
            cmd.Parameters.AddWithValue("@Status", "Pending");
            cmd.Parameters.AddWithValue("@LoanPurpose", entity.LoanPurpose ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ApplicationDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@MemberID", entity.MemberID);
            cmd.Parameters.AddWithValue("@PolicyID", entity.PolicyID);
            cmd.Parameters.AddWithValue("@GroupID", entity.GroupID ?? (object)DBNull.Value);

            conn.Open();
            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                entity.LoanID = Convert.ToInt32(result);
                return true;
            }
        }
        return false;
    }

    public override bool Update(Loan entity)
    {
        string query = @"UPDATE Loans SET AmountRequested = @AmountRequested, ApprovedAmount = @ApprovedAmount,
                        InterestRate = @InterestRate, DurationMonths = @DurationMonths, Status = @Status,
                        LoanPurpose = @LoanPurpose, ApprovalDate = @ApprovalDate, DisbursementDate = @DisbursementDate,
                        OfficerID = @OfficerID WHERE LoanID = @LoanID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LoanID", entity.LoanID);
            cmd.Parameters.AddWithValue("@AmountRequested", entity.AmountRequested);
            cmd.Parameters.AddWithValue("@ApprovedAmount", entity.ApprovedAmount ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@InterestRate", entity.InterestRate);
            cmd.Parameters.AddWithValue("@DurationMonths", entity.DurationMonths);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            cmd.Parameters.AddWithValue("@LoanPurpose", entity.LoanPurpose ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ApprovalDate", entity.ApprovalDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DisbursementDate", entity.DisbursementDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OfficerID", entity.OfficerID ?? (object)DBNull.Value);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }

    public override bool Delete(int id)
    {
        string query = "UPDATE Loans SET Status = 'Cancelled' WHERE LoanID = @LoanID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LoanID", id);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
    private void PopulateLoanData(Loan loan, SqlDataReader reader)
    {
        loan.LoanID = Convert.ToInt32(reader["LoanID"]);
        loan.AmountRequested = Convert.ToDecimal(reader["AmountRequested"]);
        loan.ApprovedAmount = reader["ApprovedAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["ApprovedAmount"]);
        loan.InterestRate = Convert.ToDecimal(reader["InterestRate"]);
        loan.DurationMonths = Convert.ToInt32(reader["DurationMonths"]);
        loan.Status = reader["Status"].ToString();
        loan.LoanPurpose = reader["LoanPurpose"].ToString();
        loan.ApplicationDate = Convert.ToDateTime(reader["ApplicationDate"]);
        loan.ApprovalDate = reader["ApprovalDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ApprovalDate"]);
        loan.DisbursementDate = reader["DisbursementDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DisbursementDate"]);
        loan.MemberID = Convert.ToInt32(reader["MemberID"]);
        loan.OfficerID = reader["OfficerID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["OfficerID"]);
        loan.PolicyID = Convert.ToInt32(reader["PolicyID"]);
        loan.GroupID = reader["GroupID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["GroupID"]);
        loan.MemberName = reader["MemberName"].ToString();
        loan.OfficerName = reader["OfficerName"].ToString();
        loan.PolicyType = reader["PolicyType"].ToString();
    }
}
