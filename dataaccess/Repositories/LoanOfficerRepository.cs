using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MicroFinance_Loan.dataaccess.Repositories
{
    public class LoanOfficerRepository : BaseRepository<LoanOfficer>
    {
        public override List<LoanOfficer> GetAll()
        {
            List<LoanOfficer> officers = new List<LoanOfficer>();

            string query = @"SELECT lo.OfficerID, lo.AssignedGroupID, lo.HireDate, 
                                    u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive
                             FROM LoanOfficers lo
                             INNER JOIN Users u ON lo.OfficerID = u.UserID
                             WHERE u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LoanOfficer officer = new LoanOfficer();
                    PopulateOfficerData(officer, reader);
                    officers.Add(officer);
                }
            }
            return officers;
        }

        public override LoanOfficer GetById(int id)
        {
            string query = @"SELECT lo.OfficerID, lo.AssignedGroupID, lo.HireDate, 
                                    u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive
                             FROM LoanOfficers lo
                             INNER JOIN Users u ON lo.OfficerID = u.UserID
                             WHERE lo.OfficerID = @OfficerID AND u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OfficerID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    LoanOfficer officer = new LoanOfficer();
                    PopulateOfficerData(officer, reader);
                    return officer;
                }
            }
            return null;
        }

        public override bool Insert(LoanOfficer entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO LoanOfficers (OfficerID, AssignedGroupID, HireDate) VALUES (@OfficerID, @AssignedGroupID, @HireDate)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OfficerID", entity.UserID);
                cmd.Parameters.AddWithValue("@AssignedGroupID", (object)entity.AssignedGroupID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HireDate", DateTime.Now);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override bool Update(LoanOfficer entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE LoanOfficers SET AssignedGroupID = @AssignedGroupID, HireDate = @HireDate WHERE OfficerID = @OfficerID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignedGroupID", (object)entity.AssignedGroupID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HireDate", entity.HireDate);
                cmd.Parameters.AddWithValue("@OfficerID", entity.UserID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM LoanOfficers WHERE OfficerID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void PopulateOfficerData(LoanOfficer officer, SqlDataReader reader)
        {
            officer.UserID = Convert.ToInt32(reader["OfficerID"]);
            officer.Name = reader["Name"].ToString();
            officer.Username = reader["Username"].ToString();
            officer.PasswordHash = reader["PasswordHash"].ToString();
            officer.Role = reader["Role"].ToString();
            officer.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            officer.IsActive = Convert.ToBoolean(reader["IsActive"]);
            officer.AssignedGroupID = reader["AssignedGroupID"] != DBNull.Value ? Convert.ToInt32(reader["AssignedGroupID"]) : (int?)null;
            officer.HireDate = Convert.ToDateTime(reader["HireDate"]);
        }
    }
}
