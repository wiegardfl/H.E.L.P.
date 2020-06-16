#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
#endregion

namespace ConnectionConfigurator.DataAccess
{
    sealed class ConfigFile
    {
        #region Variables
        private static readonly string filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\HELP.conf";
        #endregion

        #region Constructors
        private ConfigFile() {}
        #endregion

        #region Methods
        public static Dictionary<string, string> ReadConfig()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                using (var file = File.Create(filePath)) {}

                StreamWriter writer = new StreamWriter(filePath);
                string[] keys = { "server", "port", "database", "username", "password" };

                foreach (string key in keys)
                {
                    keyValues.Add(key, "");
                    writer.WriteLine(AES.Encrypt(key + "=", App.EncryptionKey));
                }

                writer.Close();

                return keyValues;
            }

            StreamReader fileContent = new StreamReader(filePath);
            string line;

            while ((line = fileContent.ReadLine()) != null)
            {
                string[] keyValue = AES.Decrypt(line, App.EncryptionKey).Split(new char[] { '=' }, 2);

                try
                {
                    keyValues.Add(keyValue[0], keyValue[1]);
                } catch (IndexOutOfRangeException e)
                {
                    continue;
                }
                
            }

            fileContent.Close();

            return keyValues;
        }

        public static void WriteConfig(string[] values)
        {
            string[] keys = { "server", "port", "database", "username", "password" };

            if (!File.Exists(filePath)) File.Create(filePath);

            StreamWriter writer = new StreamWriter(filePath);

            for (int i = 0; i< keys.Length; i++)
            {
                writer.WriteLine(AES.Encrypt(keys[i] + "=" + values[i], App.EncryptionKey));
            }

            writer.Close();
        }

        public static void RecreateCorruptConfig()
        {
            File.Delete(filePath);

            using (var file = File.Create(filePath)) {}

            StreamWriter writer = new StreamWriter(filePath);
            string[] keys = { "server", "port", "database", "username", "password" };

            foreach (string key in keys)
            {
                writer.WriteLine(AES.Encrypt(key + "=", App.EncryptionKey));
            }

            writer.Close();
        }
        #endregion
    }
}