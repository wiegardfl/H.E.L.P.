#region References
using System;
using System.Windows;

using HELP.DataModels;
#endregion


namespace HELP.Forms
{
    public partial class PatientData : Window
    {
        #region Constructors
        public PatientData()
        {
            InitializeComponent();

            this.DataContext = Data;
            this.NewPatient = true;
        }

        public PatientData(Patient patient)
        {
            InitializeComponent();

            this.Data = patient;
            this.DataCopy = patient.Clone();
            this.DataContext = this.Data;
        }
        #endregion

        #region Methods

        #region Events
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DynamicData.Patients.Count; i++)
            {
                if (this.DataCopy.PatientNr.Equals(DynamicData.Patients[i].PatientNr)) DynamicData.Patients[i] = this.DataCopy;
            }

            this.DialogResult = false;

            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.Data.Birthday.CompareTo(DateTime.Now) < 0 || this.Data.Birthday.CompareTo(DateTime.Now) == 0))
            {
                MessageBox.Show("Ungültiges Geburtsdatum!", "Geburtsdatum ungültig");

                return;
            }

            if (CheckNecessaryFields())
            {
                if (this.NewPatient && !DynamicData.Patients.Contains(this.Data))
                {
                    App.DBConnection.AddPatient(this.Data);

                    DynamicData.Patients.Add(this.Data);
                }

                if (!this.NewPatient) App.DBConnection.UpdatePatient(this.Data);

                this.DialogResult = true;

                this.Close();
            } else
            {
                MessageBox.Show("Bitte alle notwendigen Felder ausfüllen!", "Notwendige Felder ausfüllen");
            }
        }
        #endregion

        private bool CheckNecessaryFields()
        {
            if (
                this.Data.FirstName.Trim().Length != 0 &&
                this.Data.LastName.Trim().Length != 0 &&
                this.Data.Gender != null &&
                this.Data.Birthday != null &&
                this.Data.KVNR.Trim().Length != 0 &&
                this.Data.Address.Trim().Length != 0 &&
                this.Data.PostalCode.Trim().Length != 0 &&
                this.Data.City.Trim().Length != 0
                )
            {
                if (this.Data.Age < 18)
                {
                    if (
                        this.Data.FunctionRelatives.Trim().Length != 0 &&
                        this.Data.FirstNameRelatives.Trim().Length != 0 &&
                        this.Data.LastNameRelatives.Trim().Length != 0 &&
                        this.Data.AddressRelatives.Trim().Length != 0 &&
                        this.Data.PostalCodeRelatives.Trim().Length != 0 &&
                        this.Data.CityRelatives.Trim().Length != 0
                        )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Properties
        public Patient Data { get; private set; } = new Patient();
        public Patient DataCopy { get; private set; } = new Patient();
        public bool NewPatient { get; private set; } = false;
        #endregion
    }
}