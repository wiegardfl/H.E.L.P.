using HELP.DataModels;
using K4os.Hash.xxHash;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace HELP.Forms
{
    /// <summary>
    /// Interaktionslogik für Form_Patient.xaml
    /// </summary>
    public partial class Form_Patient : Window
    {
        private bool newPatient;

        public Form_Patient()
        {
            newPatient = true;

            InitializeComponent();
            DataContext = PatientData;
            Loaded += (sender, e) => Keyboard.Focus(txtVorname);
        }

        public Form_Patient(Patient patient) : this()
        {
            newPatient = false;

            Copy = patient.Clone();
            PatientData = patient;
            DataContext = PatientData;

            var text = patient.Gender.ToString();
            cmbGeschlecht.SelectedValue = text.Replace("ae", "ä");
            cmbNationalitaet.SelectedValue = patient.Nationality;
            cmbKrankenkasse.SelectedValue = patient.HealthInsurance;
            cmFunktion.SelectedValue = patient.FunctionRelatives;
        }

        private bool CheckNecessaryFields()
        {
            if (
                PatientData.FirstName.Trim().Length != 0 &&
                PatientData.LastName.Trim().Length != 0 &&
                PatientData.Gender != null &&
                PatientData.Birthday != null &&
                PatientData.KVNR.Trim().Length != 0 &&
                PatientData.Address.Trim().Length != 0 &&
                PatientData.PostalCode.Trim().Length != 0 &&
                PatientData.City.Trim().Length != 0 &&
                (PatientData.Phone.Trim().Length != 0 || PatientData.Mobile.Trim().Length != 0)
                )
            {
                if (PatientData.Age < 18)
                {
                    if (
                        PatientData.FunctionRelatives.Trim().Length != 0 &&
                        PatientData.FirstNameRelatives.Trim().Length != 0 &&
                        PatientData.LastNameRelatives.Trim().Length != 0 &&
                        PatientData.AddressRelatives.Trim().Length != 0 &&
                        PatientData.PostalCodeRelatives.Trim().Length != 0 &&
                        PatientData.CityRelatives.Trim().Length != 0 &&
                        (PatientData.PhoneRelatives.Trim().Length != 0 || PatientData.MobileRelatives.Trim().Length != 0)
                        )
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                } else
                {
                    return true;
                }
            } else
            {
                return false;
            }
        }

        public Patient PatientData { get; private set; } = new Patient();
        public Patient Copy { get; private set; } = new Patient();
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            PatientData.Gender = cmbGeschlecht.SelectedItem == null ? "" : ((ComboBoxItem)cmbGeschlecht.SelectedItem).Content.ToString().Replace("ä", "ae");
            PatientData.Nationality = cmbNationalitaet.SelectedItem == null ? "" : ((ComboBoxItem)cmbNationalitaet.SelectedItem).Content.ToString();
            PatientData.HealthInsurance = cmbKrankenkasse.SelectedItem == null ? "" : ((ComboBoxItem)cmbKrankenkasse.SelectedItem).Content.ToString();
            PatientData.FunctionRelatives = cmFunktion.SelectedItem == null ? "" : ((ComboBoxItem)cmFunktion.SelectedItem).Content.ToString();

            if (!(PatientData.Birthday.CompareTo(DateTime.Now) < 0 || PatientData.Birthday.CompareTo(DateTime.Now) == 0))
            {
                MessageBox.Show("Ungültiges Geburtsdatum!", "Geburtsdatum ungültig");

                return;
            }

            if (CheckNecessaryFields())
            {
                if (newPatient && !PatientList.patients.Contains(PatientData))
                {
                    PatientList.patients.Add(PatientData);
                }

                DialogResult = true;

                Close();
            } else
            {
                MessageBox.Show("Bitte alle notwendigen Felder ausfüllen!", "Notwendige Felder ausfüllen");
            }
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < PatientList.patients.Count; i++)
            {
                if (Copy.KVNR.Equals(PatientList.patients[i].KVNR))
                {
                    PatientList.patients[i] = Copy;
                }
            }

            DialogResult = false;

            Close();
        }
    }
}
