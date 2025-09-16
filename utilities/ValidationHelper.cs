using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFinance_Loan.utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            return phone.Length == 11 && phone.StartsWith("01") && phone.All(char.IsDigit);
        }

        public static bool IsValidNID(string nid)
        {
            if (string.IsNullOrEmpty(nid)) return false;

            return (nid.Length == 10 || nid.Length == 13 || nid.Length == 17) && nid.All(char.IsDigit);
        }

        public static bool IsValidAmount(decimal amount)
        {
            return amount > 0 && amount <= 999999999.99m;
        }

        public static bool IsValidInterestRate(decimal rate)
        {
            return rate >= 0 && rate <= 100;
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
