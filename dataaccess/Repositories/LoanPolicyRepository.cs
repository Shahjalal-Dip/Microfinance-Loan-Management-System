using Microfinance_Loan_Management_System.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.DataAccess.Repositories
{
    public class LoanPolicyRepository : BaseRepository<LoanPolicy>
    {
        public override List<LoanPolicy> GetAll()
        {
            List<LoanPolicy> policies = new List<LoanPolicy>();
            string query = @"SELECT PolicyID, LoanType, MaxAmount, InterestRate, PenaltyRate, MinDurationMonths, 
                        MaxDurationMonths, CreatedBy, CreatedDate, IsActive FROM LoanPolicies ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LoanPolicy policy = new LoanPolicy();
                    PopulatePolicyData(policy, reader);
                    policies.Add(policy);
                }
            }
            return policies;
        }

        public override LoanPolicy GetById(int id)
        {
            string query = @"SELECT PolicyID, LoanType, MaxAmount, InterestRate, PenaltyRate, MinDurationMonths, 
                        MaxDurationMonths, CreatedBy, CreatedDate, IsActive FROM LoanPolicies WHERE PolicyID = @PolicyID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    LoanPolicy policy = new LoanPolicy();
                    PopulatePolicyData(policy, reader);
                    return policy;
                }
            }
            return null;
        }

        public override bool Insert(LoanPolicy entity)
        {
            string query = @"INSERT INTO LoanPolicies (LoanType, MaxAmount, InterestRate, PenaltyRate, 
                        MinDurationMonths, MaxDurationMonths, CreatedBy, CreatedDate, IsActive) 
                        VALUES (@LoanType, @MaxAmount, @InterestRate, @PenaltyRate, @MinDurationMonths, 
                        @MaxDurationMonths, @CreatedBy, @CreatedDate, @IsActive);
                        SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LoanType", entity.LoanType);
                cmd.Parameters.AddWithValue("@MaxAmount", entity.MaxAmount);
                cmd.Parameters.AddWithValue("@InterestRate", entity.InterestRate);
                cmd.Parameters.AddWithValue("@PenaltyRate", entity.PenaltyRate);
                cmd.Parameters.AddWithValue("@MinDurationMonths", entity.MinDurationMonths);
                cmd.Parameters.AddWithValue("@MaxDurationMonths", entity.MaxDurationMonths);
                cmd.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    entity.PolicyID = Convert.ToInt32(result);
                    return true;
                }
            }
            return false;
        }

        public override bool Update(LoanPolicy entity)
        {
            string query = @"UPDATE LoanPolicies SET LoanType = @LoanType, MaxAmount = @MaxAmount, 
                        InterestRate = @InterestRate, PenaltyRate = @PenaltyRate, MinDurationMonths = @MinDurationMonths, 
                        MaxDurationMonths = @MaxDurationMonths, IsActive = @IsActive WHERE PolicyID = @PolicyID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", entity.PolicyID);
                cmd.Parameters.AddWithValue("@LoanType", entity.LoanType);
                cmd.Parameters.AddWithValue("@MaxAmount", entity.MaxAmount);
                cmd.Parameters.AddWithValue("@InterestRate", entity.InterestRate);
                cmd.Parameters.AddWithValue("@PenaltyRate", entity.PenaltyRate);
                cmd.Parameters.AddWithValue("@MinDurationMonths", entity.MinDurationMonths);
                cmd.Parameters.AddWithValue("@MaxDurationMonths", entity.MaxDurationMonths);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            } 
        }

        public override bool Delete(int id)
        {
            string query = "DELETE FROM LoanPolicies WHERE PolicyID = @PolicyID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", id);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool DeactivatePolicy(int policyId)
        {
            string query = "UPDATE LoanPolicies SET IsActive = 0 WHERE PolicyID = @PolicyID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", policyId);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        private void PopulatePolicyData(LoanPolicy policy, SqlDataReader reader)
        {
            policy.PolicyID = Convert.ToInt32(reader["PolicyID"]);
            policy.LoanType = reader["LoanType"].ToString();
            policy.MaxAmount = Convert.ToDecimal(reader["MaxAmount"]);
            policy.InterestRate = Convert.ToDecimal(reader["InterestRate"]);
            policy.PenaltyRate = Convert.ToDecimal(reader["PenaltyRate"]);
            policy.MinDurationMonths = Convert.ToInt32(reader["MinDurationMonths"]);
            policy.MaxDurationMonths = Convert.ToInt32(reader["MaxDurationMonths"]);
            policy.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
            policy.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
            policy.IsActive = Convert.ToBoolean(reader["IsActive"]);
        }
    }
}