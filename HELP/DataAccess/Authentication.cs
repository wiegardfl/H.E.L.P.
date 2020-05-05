using HELP.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Windows;

namespace HELP.DataAccess
{
    sealed class Authentication
    {
        private static readonly string HOST = "localhost";
        private static readonly string PORT = "3306";
        private static readonly string DATABASE = "HELPAuth";
        private static readonly string USERNAME = "root";
        private static readonly string PASSWORD = "password";

        private Authentication() {}

        public static int Authenticate(string[] credentials)
        {

            using (MySqlConnection connection = new MySqlConnection("SERVER=" + HOST + ";PORT=" + PORT + ";DATABASE=" + DATABASE + ";UID=" + USERNAME + ";PASSWORD=" + PASSWORD + ";"))
            {
                try
                {
                    connection.Open();

                    MySqlCommand query = new MySqlCommand("SELECT username, password, role, login_blocked, login_failed, last_failed_attempt FROM userCredentials", connection);
                    MySqlDataReader result = query.ExecuteReader();

                    while (result.Read()) {
                        if (credentials[0].Equals(result["username"]))
                        {
                            bool lockTimer = true;

                            if (!result.IsDBNull(result.GetOrdinal("login_blocked")))
                            {
                                if (DateTime.Compare(DateTime.Now, (DateTime)result["login_blocked"]) < 0)
                                {
                                    return -1;
                                }
                            }

                            if (credentials[1].Equals(result["password"]))
                            {
                                switch (result["role"])
                                {
                                    case "nurse":
                                        return 1;
                                    case "physician":
                                        return 2;
                                    case "admin":
                                        return 3;
                                    case "secretary":
                                        return 4;
                                }
                            } else
                            {
                                if (!result.IsDBNull(result.GetOrdinal("last_failed_attempt")))
                                {
                                    if (DateTime.Compare(DateTime.Now, ((DateTime)result["last_failed_attempt"]).AddMinutes(5)) >= 0)
                                    {
                                        lockTimer = false;
                                    }
                                }

                                string sql = "UPDATE userCredentials SET login_blocked=" +
                                             ((int)result["login_failed"] + 1 >= 3 ? "'" + DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL") +
                                             ",login_failed=" + (lockTimer ? ((int)result["login_failed"] + 1 >= 3 ? 0 : (int)result["login_failed"] + 1) : 1) +
                                             ",last_failed_attempt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                             "WHERE username='" + result["username"] + "'";

                                MySqlCommand command = new MySqlCommand(sql, connection);

                                result.Close();
                                command.ExecuteNonQuery();

                                return 0;
                            }
                        }
                    }

                    return 0;
                }
                catch (MySqlException ex1) {
                    MessageBox.Show(ex1.ToString());
                    return -2;
                } finally {
                    try { connection.Close(); } catch (MySqlException ex2) {}
                }
            }
        }
    }
}
