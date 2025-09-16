using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFinance_Loan.dataaccess.Repositories
{
    public class AdminRepository : BaseRepository<Admin>
    {
        public override List<Admin> GetAll() {

            List<Admin> admins = new List<Admin>();
            string query = @"SELECT a.AdminID, u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive
                        FROM Admins a
                        INNER JOIN Users u ON a.AdminID = u.UserID
                        WHERE u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Admin admin = new Admin();
                    PopulateAdminData(admin, reader);
                    admins.Add(admin);
                }
            }
            return admins;
        }
        public override Admin GetById(int id) {
            string query = @"SELECT a.AdminID, a.Department, u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive
                       
                        FROM Admins a 
                        INNER JOIN Users u ON a.AdminID = u.UserID
                        WHERE a.AdminID = @AdminID AND u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AdminID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Admin admin = new Admin();
                    PopulateAdminData(admin, reader);
                    return admin;
                }
            }
            return null;
        
        }

        public override bool Insert(Admin entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Admins (AdminID, Department) VALUES (@AdminID, @Department)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AdminID", entity.UserID);
                cmd.Parameters.AddWithValue("@Department", entity.Department);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override bool Update(Admin entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Admins SET Department = @Department WHERE AdminID = @AdminID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Department", entity.Department);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Admins WHERE AdminID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void PopulateAdminData(Admin admin, SqlDataReader reader)
        {
            admin.UserID = Convert.ToInt32(reader["AdminID"]);
            admin.Name = reader["Name"].ToString();
            admin.Username = reader["Username"].ToString();
            admin.PasswordHash = reader["PasswordHash"].ToString();
            admin.Role = reader["Role"].ToString();
            admin.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            admin.IsActive = Convert.ToBoolean(reader["IsActive"]);
            admin.Department = reader["Department"].ToString();
        }
    }
}
