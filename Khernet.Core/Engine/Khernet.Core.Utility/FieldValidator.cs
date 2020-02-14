using System.Security;
using System.Text.RegularExpressions;

namespace Khernet.Core.Utility
{
    public static class FieldValidator
    {
        /// <summary>
        /// Validates if password is valid, it must have letters, numbers and special characters.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ValidatePassword(SecureString password)
        {
            CryptographyProvider crypt = new CryptographyProvider();
            string pass = crypt.RetrieveString(password);

            bool result = !string.IsNullOrEmpty(pass);
            result &= !string.IsNullOrWhiteSpace(pass);
            result &= pass.Length > 9;
            result &= Regex.IsMatch(pass, @"[^\w]");
            result &= Regex.IsMatch(pass, @"[\w]");
            result &= Regex.IsMatch(pass, @"[0-9]");

            return result;
        }

        /// <summary>
        /// Verify if two  password objects have same values
        /// </summary>
        /// <param name="firstPassword">The fisrt password</param>
        /// <param name="secondPassword">The second password</param>
        /// <returns></returns>
        public static bool ComparePasswords(SecureString firstPassword, SecureString secondPassword)
        {
            CryptographyProvider crypt = new CryptographyProvider();
            CryptographyProvider crypt2 = new CryptographyProvider();
            return crypt.RetrieveString(firstPassword).Equals(crypt2.RetrieveString(secondPassword));
        }

        /// <summary>
        /// Validates a user name
        /// </summary>
        /// <param name="username">The username to validate</param>
        /// <returns></returns>
        public static bool ValidateUserName(string username)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length > 20)
                return false;

            //Only word characters, numbers and underscores allowed
            if (Regex.IsMatch(username, @"[^\w]"))
                return false;

            return true;
        }
    }
}
