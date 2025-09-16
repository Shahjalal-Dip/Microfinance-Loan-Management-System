using System.Data.SqlClient;

namespace Microfinance_Loan_Management_System.DataAccess
{
    public class DatabaseConnection
    {
        private static string connectionString = "Server=DESKTOP-0HB4NKT\\SQLEXPRESS;Database=MicrofinanceLoan;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}