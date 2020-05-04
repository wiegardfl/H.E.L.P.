using System.Security.Cryptography;
using System.Text;

namespace HELP.DataAccess
{
    sealed class MD5Hash
    {
        private MD5Hash() {}

        public static string HashString(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                StringBuilder stringBuilder = new StringBuilder();
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}
