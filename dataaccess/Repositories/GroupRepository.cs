using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.DataAccess.Repositories
{
    public class GroupRepository : BaseRepository<Group>
    {
        public override List<Group> GetAll()
        {
            List<Group> groups = new List<Group>();
            string query = @"SELECT GroupID, GroupName, GroupLeader, MaxMembers, CreatedDate, IsActive 
                        FROM Groups WHERE IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Group group = new Group();
                    PopulateGroupData(group, reader);
                    groups.Add(group);
                }
            }
            foreach (Group group in groups)
            {
                group.Members = GetGroupMembers(group.GroupID);
            }

            return groups;
        }

        public override Group GetById(int id)
        {
            string query = @"SELECT GroupID, GroupName, GroupLeader, MaxMembers, CreatedDate, IsActive 
                        FROM Groups WHERE GroupID = @GroupID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Group group = new Group();
                    PopulateGroupData(group, reader);

                    group.Members = GetGroupMembers(group.GroupID);

                    return group;
                }
            }
            return null;
        }

        public override bool Insert(Group entity)
        {
            string query = @"INSERT INTO Groups (GroupName, GroupLeader, MaxMembers, CreatedDate, IsActive) 
                        VALUES (@GroupName, @GroupLeader, @MaxMembers, @CreatedDate, @IsActive);
                        SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupName", entity.GroupName);
                cmd.Parameters.AddWithValue("@GroupLeader", entity.GroupLeader ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxMembers", entity.MaxMembers);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    entity.GroupID = Convert.ToInt32(result);
                    entity.CreatedDate = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        public override bool Update(Group entity)
        {
            string query = @"UPDATE Groups SET GroupName = @GroupName, GroupLeader = @GroupLeader, 
                        MaxMembers = @MaxMembers, IsActive = @IsActive WHERE GroupID = @GroupID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", entity.GroupID);
                cmd.Parameters.AddWithValue("@GroupName", entity.GroupName);
                cmd.Parameters.AddWithValue("@GroupLeader", entity.GroupLeader ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxMembers", entity.MaxMembers);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public override bool Delete(int id)
        {
            string query = "UPDATE Groups SET IsActive = 0 WHERE GroupID = @GroupID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Member> GetGroupMembers(int groupID)
        {
            List<Member> members = new List<Member>();
            string query = @"SELECT m.MemberID, u.Name, u.Username, u.PasswordHash, u.Role, u.CreatedDate, u.IsActive,
                        m.NID, m.Address, m.Phone, m.Income, m.GroupID, m.JoinDate, g.GroupName
                        FROM Members m 
                        INNER JOIN Users u ON m.MemberID = u.UserID
                        LEFT JOIN Groups g ON m.GroupID = g.GroupID
                        WHERE m.GroupID = @GroupID AND u.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", groupID);
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

        public bool AddMemberToGroup(int memberID, int groupID)
        {
            Group group = GetById(groupID);
            if (group == null) return false;

            if (group.GetCurrentMemberCount() >= group.MaxMembers)
            {
                throw new Exception("Group has reached maximum capacity");
            }

            string query = "UPDATE Members SET GroupID = @GroupID WHERE MemberID = @MemberID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", groupID);
                cmd.Parameters.AddWithValue("@MemberID", memberID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool RemoveMemberFromGroup(int memberID)
        {
            string query = "UPDATE Members SET GroupID = NULL WHERE MemberID = @MemberID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MemberID", memberID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Group> GetAvailableGroups()
        {
            List<Group> availableGroups = new List<Group>();
            string query = @"SELECT g.GroupID, g.GroupName, g.GroupLeader, g.MaxMembers, g.CreatedDate, g.IsActive,
                        COUNT(m.MemberID) as CurrentMembers
                        FROM Groups g
                        LEFT JOIN Members m ON g.GroupID = m.GroupID
                        WHERE g.IsActive = 1
                        GROUP BY g.GroupID, g.GroupName, g.GroupLeader, g.MaxMembers, g.CreatedDate, g.IsActive
                        HAVING COUNT(m.MemberID) < g.MaxMembers";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Group group = new Group();
                    PopulateGroupData(group, reader);
                    availableGroups.Add(group);
                }
            }
            return availableGroups;
        }

        public DataTable GetGroupStatistics()
        {
            string query = @"SELECT 
                        g.GroupName,
                        g.MaxMembers,
                        COUNT(m.MemberID) as CurrentMembers,
                        g.MaxMembers - COUNT(m.MemberID) as AvailableSlots,
                        COUNT(l.LoanID) as ActiveLoans,
                        SUM(CASE WHEN l.Status = 'Active' THEN l.ApprovedAmount ELSE 0 END) as TotalLoanAmount
                        FROM Groups g
                        LEFT JOIN Members m ON g.GroupID = m.GroupID
                        LEFT JOIN Loans l ON m.MemberID = l.MemberID AND l.Status IN ('Approved', 'Active')
                        WHERE g.IsActive = 1
                        GROUP BY g.GroupID, g.GroupName, g.MaxMembers";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private void PopulateGroupData(Group group, SqlDataReader reader)
        {
            group.GroupID = Convert.ToInt32(reader["GroupID"]);
            group.GroupName = reader["GroupName"].ToString();
            group.GroupLeader = reader["GroupLeader"] == DBNull.Value ? null : reader["GroupLeader"].ToString();
            group.MaxMembers = Convert.ToInt32(reader["MaxMembers"]);
            group.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            group.IsActive = Convert.ToBoolean(reader["IsActive"]);
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
            member.GroupName = reader["GroupName"].ToString();
            member.JoinDate = Convert.ToDateTime(reader["JoinDate"]);
        }
    }
}