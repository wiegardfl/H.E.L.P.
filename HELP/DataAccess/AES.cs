#region References
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
#endregion

namespace HELP.DataAccess
{
    sealed class AES
    {
        #region Constructors
        private AES() {}
        #endregion

        #region Methods
        public static string Decrypt(string cipherText, string key)
        {
            try
            {
                var allBytes = Convert.FromBase64String(cipherText);
                var saltBytes = allBytes.Take(32).ToArray();
                var cipherTextBytes = allBytes.Skip(32).Take(allBytes.Length - 32).ToArray();

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes))
                {
                    var keyBytes = keyDerivationFunction.GetBytes(32);
                    var ivBytes = keyDerivationFunction.GetBytes(16);

                    using (var aesManaged = new AesManaged())
                    using (var decryptor = aesManaged.CreateDecryptor(keyBytes, ivBytes))
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion
    }
}