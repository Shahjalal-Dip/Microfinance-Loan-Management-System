using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{
    public class AuthenticationService
    {
        private UserRepository userRepository;

        public AuthenticationService()
        {
            userRepository = new UserRepository();
        }

        public User Login(string username, string password)
        {
            try
            {
                User user = userRepository.GetByUsername(username);

                if (user != null && user.IsActive)
                {
                    string hashedPassword = HashPassword(password);
                    if (user.PasswordHash == hashedPassword)
                    {
                        return user;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed: " + ex.Message);
            }
        }

        public bool Register(User user, string password)
        {
            try
            {
                if (userRepository.GetByUsername(user.Username) != null)
                {
                    throw new Exception("Username already exists");
                }

                user.PasswordHash = HashPassword(password);
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;

                return userRepository.Insert(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Registration failed: " + ex.Message);
            }
        }

        public bool ChangePassword(int userID, string oldPassword, string newPassword)
        {
            try
            {
                User user = userRepository.GetById(userID);
                if (user == null) return false;

                string hashedOldPassword = HashPassword(oldPassword);
                if (user.PasswordHash != hashedOldPassword) return false;

                user.PasswordHash = HashPassword(newPassword);
                return userRepository.Update(user);
            }
            catch
            {
                return false;
            }
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + "MicrofinanceSalt"));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}