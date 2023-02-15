using System.Text;
using System.Security.Cryptography;

namespace BUSB
{
    internal static class CommonMethods
    {
        public static string MD5Hash(string str) => Encoding.ASCII.GetString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(str)));

        public static bool ConfirmPwd(string pwd, string realPassword)
        {
            string str = CommonMethods.MD5Hash(pwd);
            return (realPassword == str);
        }
    }
}
