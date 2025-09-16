using System.Collections.Generic;

namespace Microfinance_Loan_Management_System.DataAccess
{
    public abstract class BaseRepository<T> where T : class
    {
        protected string connectionString;

        public BaseRepository()
        {
            connectionString = DatabaseConnection.ConnectionString;
        }

        public abstract List<T> GetAll();
        public abstract T GetById(int id);
        public abstract bool Insert(T entity);
        public abstract bool Update(T entity);
        public abstract bool Delete(int id);
    }
}