using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace TMS.API.Helpers
{
    public static class UtilityHelper
    {
        public static int GenerateOTP()
        {
            int NoDigits = 4;
            Random rnd = new Random();
            int OPT = Convert.ToInt32(rnd.Next((int)Math.Pow(10, (NoDigits - 1)), (int)Math.Pow(10, NoDigits) - 1).ToString());
            return OPT;
        }

        public static string PasswordHashMaker(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash from the password string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string
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
