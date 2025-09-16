using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Microfinance_Loan_Management_System.DataAccess.Repositories
{
    
    public class MemberRepository : BaseRepository<Member>
    {
        public override List<Member> GetAll()
        {
            List<Member> members = new List<Member>();
            string query = @"SELECT m.MemberID, u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive,
                        m.NID, m.Address, m.Phone, m.Income, m.GroupID, m.JoinDate, g.GroupName
                        FROM Members m 
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN Groups g ON m.GroupID = g.GroupID
                        WHERE u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Member member = new Member();
                    PopulateMemberData(member, reader);
                    members.Add(member);
                }
            }
            return members;
        }

        public override Member GetById(int id)
        {
            string query = @"SELECT m.MemberID, u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive,
                        m.NID, m.Address, m.Phone, m.Income, m.GroupID, m.JoinDate, g.GroupName
                        FROM Members m 
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN Groups g ON m.GroupID = g.GroupID
                        WHERE m.MemberID = @MemberID AND u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MemberID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Member member = new Member();
                    PopulateMemberData(member, reader);
                    return member;
                }
            }
            return null;
        }

        public override bool Insert(Member entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    UserRepository userRepo = new UserRepository();
                    if (!InsertUser(entity, conn, transaction))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    string memberQuery = @"INSERT INTO Members (MemberID, NID, Address, Phone, Income, GroupID, JoinDate) 
                                     VALUES (@MemberID, @NID, @Address, @Phone, @Income, @GroupID, @JoinDate)";

                    SqlCommand memberCmd = new SqlCommand(memberQuery, conn, transaction);
                    memberCmd.Parameters.AddWithValue("@MemberID", entity.UserID);
                    memberCmd.Parameters.AddWithValue("@NID", entity.NID);
                    memberCmd.Parameters.AddWithValue("@Address", entity.Address ?? (object)DBNull.Value);
                    memberCmd.Parameters.AddWithValue("@Phone", entity.Phone ?? (object)DBNull.Value);
                    memberCmd.Parameters.AddWithValue("@Income", entity.Income);
                    memberCmd.Parameters.AddWithValue("@GroupID", entity.GroupID ?? (object)DBNull.Value);
                    memberCmd.Parameters.AddWithValue("@JoinDate", DateTime.Now);

                    int rowsAffected = memberCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public override bool Update(Member entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    if (!UpdateUser(entity, conn, transaction))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    string memberQuery = @"UPDATE Members SET NID = @NID, Address = @Address, Phone = @Phone, 
                                     Income = @Income, GroupID = @GroupID WHERE MemberID = @MemberID";

                    SqlCommand memberCmd = new SqlCommand(memberQuery, conn, transaction);
                    memberCmd.Parameters.AddWithValue("@MemberID", entity.UserID);
                    memberCmd.Parameters.AddWithValue("@NID", entity.NID);
                    memberCmd.Parameters.AddWithValue("@Address", entity.Address ?? (object)DBNull.Value);
                    memberCmd.Parameters.AddWithValue("@Phone", entity.Phone ?? (object)DBNull.Value);
                    memberCmd.Parameters.AddWithValue("@Income", entity.Income);
                    memberCmd.Parameters.AddWithValue("@GroupID", entity.GroupID ?? (object)DBNull.Value);

                    int rowsAffected = memberCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public override bool Delete(int id)
        {
            UserRepository userRepo = new UserRepository();
            return userRepo.Delete(id);
        }

        private void PopulateMemberData(Member member, SqlDataReader reader)
        {
            member.UserID = Convert.ToInt32(reader["MemberID"]);
            member.Name = reader["Name"].ToString();
            member.Username = reader["Username"].ToString();
            member.PasswordHash = reader["PasswordHash"].ToString();
            member.Role = reader["Role"].ToString();
            member.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            member.IsActive = Convert.ToBoolean(reader["IsActive"]);
            member.NID = reader["NID"].ToString();
            member.Address = reader["Address"].ToString();
            member.Phone = reader["Phone"].ToString();
            member.Income = Convert.ToDecimal(reader["Income"]);
            member.GroupID = reader["GroupID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["GroupID"]);
            member.GroupName = reader["GroupName"] == DBNull.Value ? null : reader["GroupName"].ToString();
            member.JoinDate = Convert.ToDateTime(reader["JoinDate"]);
        }
        
        public bool GetByNID(string nid)
        {
            string query = @"SELECT COUNT(1) FROM Members WHERE NID = @NID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NID", nid);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        private bool InsertUser(User user, SqlConnection conn, SqlTransaction transaction)
        {
            string userQuery = @"INSERT INTO Users (Name, Username, PasswordHash, Role, CreatedDate, IsActive) 
                            VALUES (@Name, @Username, @PasswordHash, @Role, @CreatedDate, @IsActive);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand userCmd = new SqlCommand(userQuery, conn, transaction);
            userCmd.Parameters.AddWithValue("@Name", user.Name);
            userCmd.Parameters.AddWithValue("@Username", user.Username);
            userCmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            userCmd.Parameters.AddWithValue("@Role", user.Role);
            userCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
            userCmd.Parameters.AddWithValue("@IsActive", user.IsActive);

            object result = userCmd.ExecuteScalar();

            if (result != null)
            {
                user.UserID = Convert.ToInt32(result);
                return true;
            }
            return false;
        }

        private bool UpdateUser(User user, SqlConnection conn, SqlTransaction transaction)
        {
            string userQuery = @"UPDATE Users SET Name = @Name, Username = @Username, 
                           PasswordHash = @PasswordHash, IsActive = @IsActive WHERE UserID = @UserID";

            SqlCommand userCmd = new SqlCommand(userQuery, conn, transaction);
            userCmd.Parameters.AddWithValue("@UserID", user.UserID);
            userCmd.Parameters.AddWithValue("@Name", user.Name);
            userCmd.Parameters.AddWithValue("@Username", user.Username);
            userCmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            userCmd.Parameters.AddWithValue("@IsActive", user.IsActive);

            int rowsAffected = userCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool InsertMember(Member entity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string memberQuery = @"INSERT INTO Members (MemberID, NID, Address, Phone, Income, GroupID, JoinDate) 
                                     VALUES (@MemberID, @NID, @Address, @Phone, @Income, @GroupID, @JoinDate)";

                SqlCommand memberCmd = new SqlCommand(memberQuery, conn);
                memberCmd.Parameters.AddWithValue("@MemberID", entity.UserID);
                memberCmd.Parameters.AddWithValue("@NID", entity.NID);
                memberCmd.Parameters.AddWithValue("@Address", entity.Address ?? (object)DBNull.Value);
                memberCmd.Parameters.AddWithValue("@Phone", entity.Phone ?? (object)DBNull.Value);
                memberCmd.Parameters.AddWithValue("@Income", entity.Income);
                memberCmd.Parameters.AddWithValue("@GroupID", entity.GroupID ?? (object)DBNull.Value);
                memberCmd.Parameters.AddWithValue("@JoinDate", DateTime.Now);
                conn.Open();
                return memberCmd.ExecuteNonQuery() > 0;
            }
        }
    }
}