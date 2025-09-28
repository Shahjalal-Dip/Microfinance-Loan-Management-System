using MicroFinance_Loan.dataaccess.Repositories;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{
    public class UserService
    {
        private UserRepository userRepository;
        private MemberRepository memberRepository;
        private AdminRepository adminRepository;

        public UserService()
        {
            userRepository = new UserRepository();
            memberRepository = new MemberRepository();
            adminRepository = new AdminRepository();
        }

        public List<User> GetAllUsers()
        {
            return userRepository.GetAll();
        }

        public List<Member> GetAllMembers()
        {
            return memberRepository.GetAll();
        }

        public bool CreateMember(Member member, string password)
        {
            try
            {
                member.PasswordHash = HashPassword(password);
                member.CreatedDate = DateTime.Now;
                member.IsActive = true;
                member.JoinDate = DateTime.Now;

                return memberRepository.Insert(member);
            }
            catch (Exception ex)
            {
                throw new Exception("Member creation failed: " + ex.Message);
            }
        }

        public bool CreateAdmin(Admin admin, string password)
        {
            try
            {
                admin.PasswordHash = HashPassword(password);
                admin.CreatedDate = DateTime.Now;
                admin.IsActive = true;
                return userRepository.Insert(admin) && adminRepository.Insert(admin);
            }
            catch (Exception ex)
            {
                throw new Exception("Admin creation failed: " + ex.Message);
            }
        }

        public bool UpdateMember(Member member)
        {
            return memberRepository.Update(member);
        }

        public bool DeactivateUser(int userID)
        {
            return userRepository.Delete(userID); // Soft delete
        }

        private string HashPassword(string password)
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