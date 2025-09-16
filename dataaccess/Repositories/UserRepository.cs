using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public override List<User> GetAll()
        {
            List<User> users = new List<User>();
            string query = @"SELECT UserID, Name, Username, PasswordHash, Role, CreatedDate, IsActive FROM Users Where IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    User user = CreateUserByRole(reader["Role"].ToString());
                    PopulateUserData(user, reader);
                    users.Add(user);
                }
            }
            return users;
        }
        public override User GetById(int id)
        {
            string query = @"SELECT UserID, Name, Username, PasswordHash, Role, CreatedDate, IsActive FROM Users WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    User user = CreateUserByRole(reader["Role"].ToString());
                    PopulateUserData(user, reader);
                    return user;
                }
            }
            return null;
        }

        public List<User> GetByRole(string role)
        {
            List<User> users = new List<User>();
            string query = @"SELECT UserID, Name, Username, PasswordHash, Role, CreatedDate, IsActive FROM Users WHERE Role = @Role";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Role", role);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User user = CreateUserByRole(reader["Role"].ToString());
                    PopulateUserData(user, reader);
                    users.Add(user);
                }
            }
            return users;
        }

        public User GetByUsername(string username)
        {
            string query = @"SELECT UserID, Name, Username, PasswordHash, Role, CreatedDate, IsActive FROM Users WHERE Username = @Username";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    User user = CreateUserByRole(reader["Role"].ToString());
                    PopulateUserData(user, reader);
                    return user;
                }
            }
            return null;
        }

        public override bool Insert(User entity)
        {
            string query = @"INSERT INTO Users (Name, Username, PasswordHash, Role, CreatedDate, IsActive) 
                        VALUES (@Name, @Username, @PasswordHash, @Role, @CreatedDate, @IsActive);
                        SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", entity.Name);
                cmd.Parameters.AddWithValue("@Username", entity.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", entity.Role);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    entity.UserID = Convert.ToInt32(result);
                    return true;
                }
            }
            return false;
        }

        public override bool Update(User entity)
        {
            string query = @"UPDATE Users SET Name = @Name, Username = @Username, 
                        PasswordHash = @PasswordHash, IsActive = @IsActive WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", entity.UserID);
                cmd.Parameters.AddWithValue("@Name", entity.Name);
                cmd.Parameters.AddWithValue("@Username", entity.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public override bool Delete(int id)
        {
            string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        private User CreateUserByRole(string role)
        {
            switch (role.ToLower())
            {
                case "member": return new Member();
                case "loanofficer": return new LoanOfficer();
                case "admin": return new Admin();
                default: throw new ArgumentException("Invalid role");
            }
        }

        private void PopulateUserData(User user, SqlDataReader reader)
        {
            user.UserID = Convert.ToInt32(reader["UserID"]);
            user.Name = reader["Name"].ToString();
            user.Username = reader["Username"].ToString();
            user.PasswordHash = reader["PasswordHash"].ToString();
            user.Role = reader["Role"].ToString();
            user.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            user.IsActive = Convert.ToBoolean(reader["IsActive"]);
        }
    }
}