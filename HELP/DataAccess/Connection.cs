#region References
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Threading;

using MySql.Data.MySqlClient;

using HELP.DataModels;
using System.Windows;
#endregion

namespace HELP.DataAccess
{
    public sealed class Connection : Window
    {
        #region Variables
        private Dictionary<string, string> ConnectionProperties = new Dictionary<string, string>();
        private MySqlConnection connection;
        private MySqlConnection dataLoaderConnection;
        private BackgroundWorker dynamicDataLoader = new BackgroundWorker();
        private bool continueDynamicDataLoading = false;
        #endregion

        #region Constructors
        public Connection() {}
        #endregion

        #region Methods
        public int InitConnection()
        {
            string filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\HELP.conf";

            if (!File.Exists(filePath)) return 1;

            StreamReader reader = new StreamReader(filePath);
            string[] properties = { "server", "port", "database", "username", "password" };
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] keyValue = AES.Decrypt(line, App.EncryptionKey).Split(new char[] { '=' }, 2);

                try
                {
                    ConnectionProperties.Add(keyValue[0], keyValue[1]);
                } catch (IndexOutOfRangeException e)
                {
                    continue;
                }
            }

            foreach (string property in properties)
            {
                if (!ConnectionProperties.ContainsKey(property)) return 2;
            }

            try
            {
                connection = new MySqlConnection("SERVER=" + ConnectionProperties["server"] + ";PORT=" + ConnectionProperties["port"] + ";DATABASE=" + ConnectionProperties["database"] + ";UID=" + ConnectionProperties["username"] + ";PASSWORD=" + ConnectionProperties["password"] + ";");
                dataLoaderConnection = new MySqlConnection("SERVER=" + ConnectionProperties["server"] + ";PORT=" + ConnectionProperties["port"] + ";DATABASE=" + ConnectionProperties["database"] + ";UID=" + ConnectionProperties["username"] + ";PASSWORD=" + ConnectionProperties["password"] + ";");

                connection.Open();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) return 3;
                else return 4;
            } finally
            {
                connection.Close();
            }

            LoadStaticData();

            dynamicDataLoader.DoWork += DynamicDataLoader_DoWork;

            return 0;
        }

        public int Authenticate(string[] credentials)
        {
            try
            {
                connection.Open();

                MySqlCommand query = new MySqlCommand("SELECT id, firstname, lastname, username, password, role, login_blocked, login_failed, last_failed_attempt FROM users", connection);
                MySqlDataReader result = query.ExecuteReader();

                while (result.Read())
                {
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
                            string fullName = result.GetString("firstname") + " " + result.GetString("lastname");

                            switch (result.GetString("role"))
                            {
                                case "nurse":
                                    App.Role = 1;
                                    App.FullNameUser = fullName;
                                    App.UserID = (int)result["id"];

                                    StartDynamicDataLoading();

                                    return 1;
                                case "medical":
                                    App.Role = 2;
                                    App.FullNameUser = fullName;
                                    App.UserID = (int)result["id"];

                                    StartDynamicDataLoading();

                                    return 2;
                                case "admin":
                                    App.Role = 3;
                                    App.FullNameUser = fullName;

                                    return 3;
                            }
                        }
                        else
                        {
                            if (!result.IsDBNull(result.GetOrdinal("last_failed_attempt")))
                            {
                                if (DateTime.Compare(DateTime.Now, ((DateTime)result["last_failed_attempt"]).AddMinutes(5)) >= 0)
                                {
                                    lockTimer = false;
                                }
                            }

                            string sql = "UPDATE users SET login_blocked=" +
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
            catch (MySqlException e)
            {
                return -2;
            } finally
            {
                connection.Close();
            }
        }

        public void LoadStaticData()
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT priority, reevaluation_time FROM priorities", connection);

                // Priorities
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.Priorities.Add(reader.GetString("priority"), reader.GetInt32("reevaluation_time"));
                        DynamicData.FilterValues.Add(reader.GetString("priority"), true);
                    }
                }

                command.CommandText = "SELECT status FROM statuses";

                // Statuses
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.Statuses.Add(reader.GetString("status"));
                        DynamicData.FilterValues.Add(reader.GetString("status"), true);
                    }
                }

                command.CommandText = "SELECT location FROM locations";

                // Locations
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.Locations.Add(reader.GetString("location"));
                        DynamicData.FilterValues.Add(reader.GetString("location"), true);
                    }
                }

                command.CommandText = "SELECT nationality FROM nationalities";

                // Nationalities
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.Nationalities.Add(reader.GetString("nationality"));
                    }
                }

                command.CommandText = "SELECT health_insurance FROM health_insurances";

                // Health Insurances
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.HealthInsurances.Add(reader.GetString("health_insurance"));
                    }
                }
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void UpdateReevaluationTimer(long caseNr, DateTime time)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("UPDATE cases SET reevaluation='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id = " + caseNr, connection);

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void AddCase(Case systemCase, List<VitalParameters> vitalParameters)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("INSERT INTO cases (patient_id, priority, status, location, arrival, reevaluation, complaint, type_of_arrival, trauma, case_status, nurse_id) VALUES (" +
                     systemCase.Data.PatientNr + "," +
                     "'" + systemCase.Priority + "'," +
                     "'" + systemCase.Status + "'," +
                     "'" + systemCase.Location + "'," +
                     "'" + systemCase.Arrival.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                     "'" + systemCase.Reevaluation.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                     "'" + systemCase.Complaint + "'," +
                     "'" + systemCase.TypeOfArrival + "'," +
                     "'" + systemCase.Trauma + "'," +
                     "'Open', " + systemCase.NurseId + "); SELECT last_insert_id();", connection
                    );

                var caseNr = Convert.ToInt32(command.ExecuteScalar());
                systemCase.CaseNr = caseNr;

                foreach (VitalParameters vital in vitalParameters)
                {
                    string sql = "INSERT INTO vital_parameters (case_id, time, heart_frequence, breath_frequence, bloodpressure_min, bloodpressure_max, temperature, oxygen_saturation) VALUES (";

                    sql += caseNr + ",";
                    sql += "'" + vital.Time.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    sql += vital.HeartFrequence == null ? "NULL," : vital.HeartFrequence + ",";
                    sql += vital.BreathFrequence == null ? "NULL," : vital.BreathFrequence + ",";
                    sql += vital.BloodPressureMin == null ? "NULL," : vital.BloodPressureMin + ",";
                    sql += vital.BloodPressureMax == null ? "NULL," : vital.BloodPressureMax + ",";
                    sql += vital.Temperature == null ? "NULL," : vital.Temperature.ToString().Replace(",", ".") + ",";
                    sql += vital.OxygenSaturation == null ? "NULL" : vital.OxygenSaturation + "";
                    sql += ")";

                    MySqlCommand vitalsCommand = new MySqlCommand(sql, connection);

                    vitalsCommand.ExecuteNonQuery();
                }
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void UpdateCase(Case systemCase)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("UPDATE cases SET priority='" + systemCase.Priority +
                     "', status='" + systemCase.Status +
                     "', location='" + systemCase.Location +
                     "', other_informations='" + systemCase.OtherInformations +
                     "', anamnesis='" + systemCase.Anamnesis +
                     "', services='" + systemCase.Services +
                     "', external_services='" + systemCase.ExternalServices +
                     "', physician_letter='" + systemCase.PhysicianLetter +
                     "', diagnosis='" + systemCase.Diagnosis +
                     "', type_of_release='" + systemCase.TypeOfRelease +
                     "'" + (systemCase.MedicalId == 0 ? "" : ", medical_id=" + systemCase.MedicalId) + " WHERE id = " + systemCase.CaseNr, connection
                    );

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void CloseCase(Case systemCase)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("UPDATE cases SET `release`='" + systemCase.Released.ToString("yyyy-MM-dd HH:mm:ss") + "', case_status='Closed' WHERE id = " + systemCase.CaseNr, connection);

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void AddPatient(Patient patient)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("INSERT INTO patients (firstname, lastname, gender, birthday, place_of_birth, nationality, health_insurance, kvnr, address, postalcode, city, phone, mobile, additional_informations, function_relatives, firstname_relatives, lastname_relatives, address_relatives, postalcode_relatives, city_relatives, phone_relatives, mobile_relatives) VALUES(" +
                     "'" + patient.FirstName + "'," +
                     "'" + patient.LastName + "'," +
                     "'" + patient.Gender + "'," +
                     "'" + patient.Birthday.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                     "'" + patient.PlaceOfBirth + "'," +
                     "'" + patient.Nationality + "'," +
                     "'" + patient.HealthInsurance + "'," +
                     "'" + patient.KVNR + "'," +
                     "'" + patient.Address + "'," +
                     "'" + patient.PostalCode + "'," +
                     "'" + patient.City + "'," +
                     "'" + patient.Phone + "'," +
                     "'" + patient.Mobile + "'," +
                     "'" + patient.AdditionalInformations + "'," +
                     "'" + patient.FunctionRelatives + "'," +
                     "'" + patient.FirstNameRelatives + "'," +
                     "'" + patient.LastNameRelatives + "'," +
                     "'" + patient.AddressRelatives + "'," +
                     "'" + patient.PostalCodeRelatives + "'," +
                     "'" + patient.CityRelatives + "'," +
                     "'" + patient.PhoneRelatives + "'," +
                     "'" + patient.MobileRelatives + "'); SELECT last_insert_id();", connection
                    );

                var patientNr = Convert.ToInt32(command.ExecuteScalar());
                patient.PatientNr = patientNr;
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void UpdatePatient(Patient patient)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("UPDATE patients SET firstname='" +
                     patient.FirstName + "', lastname='" +
                     patient.LastName + "', gender='" +
                     patient.Gender + "', birthday='" +
                     patient.Birthday.ToString("yyyy-MM-dd HH:mm:ss") + "', place_of_birth='" +
                     patient.PlaceOfBirth + "', nationality='" +
                     patient.Nationality + "', health_insurance='" +
                     patient.HealthInsurance + "', kvnr='" +
                     patient.KVNR + "', address='" +
                     patient.Address + "', postalcode='" +
                     patient.PostalCode + "', city='" +
                     patient.City + "', phone='" +
                     patient.Phone + "', mobile='" +
                     patient.Mobile + "', additional_informations='" +
                     patient.AdditionalInformations + "', function_relatives='" +
                     patient.FunctionRelatives + "', firstname_relatives='" +
                     patient.FirstNameRelatives + "', lastname_relatives='" +
                     patient.LastNameRelatives + "', address_relatives='" +
                     patient.AddressRelatives + "', postalcode_relatives='" +
                     patient.PostalCodeRelatives + "', city_relatives='" +
                     patient.CityRelatives + "', phone_relatives='" +
                     patient.Phone + "', mobile_relatives='" +
                     patient.Mobile + "' WHERE id = " + patient.PatientNr, connection
                    );

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void GetUsers()
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT id, firstname, lastname, username, role FROM users", connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DynamicData.Users.Add(new User() { ID = (int)reader["id"], FirstName = reader["firstname"].ToString(), LastName = reader["lastname"].ToString(), UserName = reader["username"].ToString(), Role = reader["role"].ToString() });
                    }
                }
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void UpdateUser(User user, bool resetPassword)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("UPDATE users SET firstname='" +
                    user.FirstName + "', lastname='" +
                    user.LastName + "', username='" +
                    user.UserName + "', role='" +
                    user.Role + "'" +
                    (resetPassword ? ", password='" + MD5Hash.HashString("password") + "'" : "") +
                    " WHERE id = " + user.ID, connection
                    );

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void AddUser(User user)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand
                    ("INSERT INTO users (firstname, lastname, username, password, role) VALUES (" +
                     "'" + user.FirstName + "'," +
                     "'" + user.LastName + "'," +
                     "'" + user.UserName + "'," +
                     "'" + MD5Hash.HashString("password") + "'," +
                     "'" + user.Role + "'); SELECT last_insert_id();", connection
                    );

                var userID = Convert.ToInt32(command.ExecuteScalar());
                user.ID = userID;
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void RemoveUser(User user)
        {
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("DELETE FROM users WHERE id = " + user.ID, connection);

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void AddVitalParameters(long caseNr, VitalParameters vitalParameters)
        {
            try
            {
                connection.Open();

                string sql = "INSERT INTO vital_parameters (case_id, time, heart_frequence, breath_frequence, bloodpressure_min, bloodpressure_max, temperature, oxygen_saturation) VALUES (";

                sql += caseNr + ",";
                sql += "'" + vitalParameters.Time.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                sql += vitalParameters.HeartFrequence == null ? "NULL," : vitalParameters.HeartFrequence + ",";
                sql += vitalParameters.BreathFrequence == null ? "NULL," : vitalParameters.BreathFrequence + ",";
                sql += vitalParameters.BloodPressureMin == null ? "NULL," : vitalParameters.BloodPressureMin + ",";
                sql += vitalParameters.BloodPressureMax == null ? "NULL," : vitalParameters.BloodPressureMax + ",";
                sql += vitalParameters.Temperature == null ? "NULL," : vitalParameters.Temperature.ToString().Replace(",", ".") + ",";
                sql += vitalParameters.OxygenSaturation == null ? "NULL" : vitalParameters.OxygenSaturation + "";
                sql += ")";

                MySqlCommand command = new MySqlCommand(sql, connection);

                command.ExecuteNonQuery();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            } finally
            {
                connection.Close();
            }
        }

        public void StartDynamicDataLoading()
        {
            continueDynamicDataLoading = true;

            try
            {
                dataLoaderConnection.Open();
            } catch (MySqlException e)
            {
                if (e.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");

                dataLoaderConnection.Close();

                return;
            }

            dynamicDataLoader.RunWorkerAsync();
        }

        public void StopDynamicDataLoading()
        {
            continueDynamicDataLoading = false;

            dynamicDataLoader.Dispose();
        }

        private void DynamicDataLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Patient> patients = new List<Patient>();
            List<Case> cases = new List<Case>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM patients", dataLoaderConnection);

            try
            {
                while (true)
                {
                    if (!App.Editing)
                    {
                        patients = new List<Patient>();
                        cases = new List<Case>();

                        Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(ClearData), new object());

                        command.CommandText = "SELECT * FROM patients";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ((int)reader["id"] == 1) continue;

                                Patient patient = new Patient()
                                {
                                    PatientNr = (int)reader["id"],
                                    FirstName = reader["firstname"].ToString(),
                                    LastName = reader["lastname"].ToString(),
                                    Gender = reader["gender"].ToString(),
                                    Birthday = DateTime.Parse(reader["birthday"].ToString()),
                                    PlaceOfBirth = "" + reader["place_of_birth"], // Nullable
                                    Nationality = "" + reader["nationality"], // Nullable
                                    HealthInsurance = "" + reader["health_insurance"], // Nullable
                                    KVNR = reader["kvnr"].ToString(),
                                    Address = reader["address"].ToString(),
                                    PostalCode = reader["postalcode"].ToString(),
                                    City = reader["city"].ToString(),
                                    Phone = "" + reader["phone"], // Nullable
                                    Mobile = "" + reader["mobile"], // Nullable
                                    AdditionalInformations = "" + reader["additional_informations"], // Nullable
                                    FunctionRelatives = "" + reader["function_relatives"], // Nullable
                                    FirstNameRelatives = "" + reader["firstname_relatives"], // Nullable
                                    LastNameRelatives = "" + reader["lastname_relatives"], // Nullable
                                    AddressRelatives = "" + reader["address_relatives"], // Nullable
                                    PostalCodeRelatives = "" + reader["postalcode_relatives"], // Nullable
                                    CityRelatives = "" + reader["city_relatives"], // Nullable
                                    PhoneRelatives = "" + reader["phone_relatives"], // Nullable
                                    MobileRelatives = "" + reader["mobile_relatives"] // Nullable
                                };

                                //Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(ClearData), new object());
                                patients.Add(patient);
                                Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(AddPatient), patient);
                                //DynamicData.Patients.Add(patient);
                            }
                        }

                        command.CommandText = "SELECT * FROM cases";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ("Closed".Equals(reader["case_status"].ToString())) continue;

                                Patient patient = new Patient() { PatientNr = 1, FirstName = "Unbekannter", LastName = "Patient" };

                                foreach (Patient pat in patients)
                                {
                                    if (pat.PatientNr == (int)reader["patient_id"]) patient = pat;
                                }

                                Case systemCase = new Case(patient)
                                {
                                    CaseNr = (int)reader["id"],
                                    Priority = reader["priority"].ToString(),
                                    Status = reader["status"].ToString(),
                                    Location = reader["location"].ToString(),
                                    Arrival = DateTime.Parse(reader["arrival"].ToString()),
                                    Reevaluation = DateTime.Parse(reader["reevaluation"].ToString()),
                                    Complaint = reader["complaint"].ToString(),
                                    TypeOfArrival = reader["type_of_arrival"].ToString(),
                                    PlaceOfIncident = "" + reader["place_of_incident"], // Nullable
                                    Trauma = reader["trauma"].ToString(),
                                    OtherInformations = "" + reader["other_informations"], // Nullable
                                    Anamnesis = "" + reader["anamnesis"], // Nullable
                                    Services = "" + reader["services"], // Nullable
                                    ExternalServices = "" + reader["external_services"], // Nullable
                                    PhysicianLetter = "" + reader["physician_letter"], // Nullable
                                    Diagnosis = "" + reader["diagnosis"], // Nullable
                                    TypeOfRelease = "" + reader["type_of_release"] // Nullable
                                };

                                //if (reader["release"] == typeof(DBNull)) systemCase.Released = DateTime.Parse(reader["release"].ToString());
                                if (!reader.IsDBNull(19)) systemCase.MedicalId = (int)reader["medical_id"];
                                if (!reader.IsDBNull(20)) systemCase.NurseId = (int)reader["nurse_id"];

                                cases.Add(systemCase);
                                Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(AddCase), systemCase);
                                //DynamicData.Cases.Add(systemCase);
                            }
                        }

                        command.CommandText = "SELECT id, firstname, lastname FROM users";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                foreach (Case sCase in cases)
                                {
                                    if (sCase.MedicalId == (int)reader["id"])
                                    {
                                        sCase.MedicalFullName = reader["firstname"].ToString() + " " + reader["lastname"].ToString();
                                    }

                                    if (sCase.NurseId == (int)reader["id"])
                                    {
                                        sCase.NurseFullName = reader["firstname"].ToString() + " " + reader["lastname"].ToString();
                                    }
                                }
                            }
                        }

                        command.CommandText = "SELECT * FROM vital_parameters";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                foreach (Case sCase in cases)
                                {
                                    if (sCase.CaseNr == (int)reader["case_id"])
                                    {
                                        VitalParameters vitalParameters = new VitalParameters() { Time = DateTime.Parse(reader["time"].ToString()) };

                                        if (!reader.IsDBNull(2)) vitalParameters.HeartFrequence = (int)reader["heart_frequence"];
                                        if (!reader.IsDBNull(3)) vitalParameters.BreathFrequence = (int)reader["breath_frequence"];
                                        if (!reader.IsDBNull(4)) vitalParameters.BloodPressureMin = (int)reader["bloodpressure_min"];
                                        if (!reader.IsDBNull(5)) vitalParameters.BloodPressureMax = (int)reader["bloodpressure_max"];
                                        if (!reader.IsDBNull(6)) vitalParameters.Temperature = (double)reader["temperature"];
                                        if (!reader.IsDBNull(7)) vitalParameters.OxygenSaturation = (int)reader["oxygen_saturation"];

                                        sCase.PreviousVitalParameters.Add(vitalParameters);
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < 39; i++)
                        {
                            if (!continueDynamicDataLoading)
                            {
                                dataLoaderConnection.Close();

                                return;
                            }

                            Thread.Sleep(500);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 39; i++)
                        {
                            if (!continueDynamicDataLoading)
                            {
                                dataLoaderConnection.Close();

                                return;
                            }

                            Thread.Sleep(500);
                        }
                    }
                }
            } catch (MySqlException ex)
            {
                if (ex.Message.ToLower().Contains("unable to connect to any of the specified mysql hosts")) MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");

                dataLoaderConnection.Close();

                return;
            }
        }

        private void ClearData(object item)
        {
            DynamicData.Patients.Clear();
            DynamicData.Cases.Clear();
        }

        private void AddPatient(object item) => DynamicData.Patients.Add((Patient)item);

        private void AddCase(object item) => DynamicData.Cases.Add((Case)item);
        #endregion
    }
}