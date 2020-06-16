#region References
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HELP.DataModels;
using HELP.Forms;
#endregion

namespace HELP.MainWindows
{
    public partial class CaseWindow : Window
    {
        #region Constructors
        public CaseWindow()
        {
            InitializeComponent();

            EnableRoleSpecificRegions();

            this.NewCase = true;

            this.txtComplaint.IsEnabled = true;
            this.txtTypeOfArrival.IsEnabled = true;
            this.txtPlaceOfIncident.IsEnabled = true;
            this.txtTrauma.IsEnabled = true;

            App.Editing = true;

            Closed += (sender, e) => App.Editing = false;
        }

        public CaseWindow(Case listCase)
        {
            InitializeComponent();

            EnableRoleSpecificRegions();

            this.SystemCase = listCase;
            this.CaseCopy = listCase.Clone();
            this.DataContext = this.SystemCase;
            this.btnSearchPatient.IsEnabled = false;
            this.btnEditData.IsEnabled = true;
            this.checkPatientAnonymous.IsEnabled = false;
            this.btnAddNewParameters.IsEnabled = true;
            this.btnReleasePatient.IsEnabled = App.Role == 1 ? false : true;

            if (this.SystemCase.Data.PatientNr == 1) this.btnEditData.IsEnabled = false;

            App.Editing = true;

            Closed += (sender, e) => App.Editing = false;
        }
        #endregion

        #region Methods

        #region Events
        private void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            PatientList selectPatient = new PatientList();

            if (selectPatient.ShowDialog() == true)
            {
                this.SystemCase = new Case(selectPatient.SelectedPatient) { Status = "Wartet", Location = "Wartezimmer" };
                this.DataContext = this.SystemCase;
                this.btnEditData.IsEnabled = true;
                this.btnAddNewParameters.IsEnabled = true;
            }
        }

        private void CheckPatientAnonymous_Changed(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                this.SystemCase = new Case(new Patient() { PatientNr = 1, FirstName = "Unbekannter", LastName = "Patient" }) { AnonymousPatient = true };
                this.DataContext = this.SystemCase;
                this.btnSearchPatient.IsEnabled = false;
                this.btnEditData.IsEnabled = false;
                this.btnAddNewParameters.IsEnabled = true;
            }
            else
            {
                this.SystemCase = null;
                this.DataContext = null;
                this.btnSearchPatient.IsEnabled = true;
                this.btnAddNewParameters.IsEnabled = false;
            }
        }
        private void CmbVitalParametersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (VitalParameters vitalParameters in SystemCase.PreviousVitalParameters)
            {
                if (vitalParameters.Time.ToString("dd.MM.yyyy HH:mm").Equals(((ComboBox)sender).SelectedItem))
                {
                    this.txtHeartFrequence.Text = vitalParameters.HeartFrequence.ToString();
                    this.txtBreathFrequence.Text = vitalParameters.BreathFrequence.ToString();
                    this.txtBloodPressureMin.Text = vitalParameters.BloodPressureMin.ToString();
                    this.txtBloodPressureMax.Text = vitalParameters.BloodPressureMax.ToString();
                    this.txtTemperature.Text = vitalParameters.Temperature.ToString();
                    this.txtOxygenSaturation.Text = vitalParameters.OxygenSaturation.ToString();

                    EnableVitalParameters(false);
                }
            }
        }

        private void BtnAddNewParameters_Click(object sender, RoutedEventArgs e)
        {
            EnableVitalParameters(true);
            SetVitalParametersFieldsToDefault();

            this.cmbVitalParametersBox.ItemsSource = null;
            this.cmbVitalParametersBox.Items.Add("Neue Vitalparameter");
            this.cmbVitalParametersBox.SelectedItem = "Neue Vitalparameter";
        }

        private void BtnCancelParameters_Click(object sender, RoutedEventArgs e)
        {
            EnableVitalParameters(false);
            SetVitalParametersFieldsToDefault();

            this.cmbVitalParametersBox.Items.Clear();
            this.cmbVitalParametersBox.ItemsSource = SystemCase.VitalParametersTimes;

            if (SystemCase.PreviousVitalParameters.Count > 0) this.cmbVitalParametersBox.SelectedIndex = 0;
        }

        private void BtnSaveParameters_Click(object sender, RoutedEventArgs e)
        {
            if (txtBloodPressureMin.Text.Trim().Length > 0 && txtBloodPressureMax.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bitte geben Sie den systolischen Blutdruck an!", "Fehlende Werte");

                return;
            } else if (txtBloodPressureMax.Text.Trim().Length > 0 && txtBloodPressureMin.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bitte geben Sie den diastolischen Blutdruck an!", "Fehlende Werte");

                return;
            }

            foreach (VitalParameters vitalParameters in SystemCase.PreviousVitalParameters)
            {
                if (vitalParameters.Time.ToString("ddMMyyyy HH:mm").Equals(DateTime.Now.ToString("ddMMyyyy HH:mm")))
                {
                    MessageBox.Show("Es exisitieren bereits Werte für diesen Zeitraum!", "Werte bereits vorhanden!");

                    return;
                }
            }

            VitalParameters newVitalParameters = new VitalParameters();

            if (this.txtHeartFrequence.Text.Trim().Length == 0) { newVitalParameters.HeartFrequence = null; } else { newVitalParameters.HeartFrequence = int.Parse(this.txtHeartFrequence.Text); }
            if (this.txtBreathFrequence.Text.Trim().Length == 0) { newVitalParameters.BreathFrequence = null; } else { newVitalParameters.BreathFrequence = int.Parse(this.txtBreathFrequence.Text); }
            if (this.txtBloodPressureMin.Text.Trim().Length == 0) { newVitalParameters.BloodPressureMin = null; } else { newVitalParameters.BloodPressureMin = int.Parse(this.txtBloodPressureMin.Text); }
            if (this.txtBloodPressureMax.Text.Trim().Length == 0) { newVitalParameters.BloodPressureMax = null; } else { newVitalParameters.BloodPressureMax = int.Parse(this.txtBloodPressureMax.Text); }
            if (this.txtTemperature.Text.Trim().Length == 0) { newVitalParameters.Temperature = null; } else { newVitalParameters.Temperature = double.Parse(this.txtTemperature.Text.Replace(".", ",")); }
            if (this.txtOxygenSaturation.Text.Trim().Length == 0) { newVitalParameters.OxygenSaturation = null; } else { newVitalParameters.OxygenSaturation = int.Parse(this.txtOxygenSaturation.Text); }

            if (!this.NewCase) App.DBConnection.AddVitalParameters(this.SystemCase.CaseNr, newVitalParameters);

            this.SystemCase.PreviousVitalParameters.Add(newVitalParameters);

            this.cmbVitalParametersBox.Items.Clear();
            this.cmbVitalParametersBox.ItemsSource = SystemCase.VitalParametersTimes;
            this.cmbVitalParametersBox.SelectedIndex = 0;
        }

        private void BtnEditData_Click(object sender, RoutedEventArgs e)
        {
            Patient dataClone = SystemCase.Data.Clone();
            PatientData editPatientData = new PatientData(SystemCase.Data);

            if (editPatientData.ShowDialog() == false)
            {
                foreach (Patient patient in DynamicData.Patients)
                {
                    if (dataClone.KVNR.Equals(patient.KVNR))
                    {
                        this.SystemCase.Data = patient;

                        break;
                    }
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DynamicData.Cases.Count; i++)
            {
                if (this.CaseCopy.CaseNr.Equals(DynamicData.Cases[i].CaseNr)) DynamicData.Cases[i] = this.CaseCopy;

                this.Close();
            }
        }

        private void BtnReleasePatient_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtDiagnosis.Text.Trim().Length == 0 || this.cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Für die Entlassung müssen die Diangnose und die Art der Entlassung angeben sein!", "Fehlende Werte");

                return;
            }

            this.SystemCase.Released = DateTime.Now;
            this.SystemCase.CaseStatus = "Closed";

            App.DBConnection.UpdateCase(this.SystemCase);
            App.DBConnection.CloseCase(this.SystemCase);

            DynamicData.Cases.Remove(this.SystemCase);

            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckNecessaryFields())
            {
                MessageBox.Show("Bitte füllen Sie alle notwendingen Felder aus!", "Fehlende Werte");

                return;
            }

            if (this.NewCase)
            {
                this.SystemCase.NurseId = App.UserID;
                this.SystemCase.NurseFullName = App.FullNameUser;
                this.SystemCase.Reevaluation = this.SystemCase.Arrival.AddMinutes(this.SystemCase.PriorityInt);

                DynamicData.Cases.Add(this.SystemCase);

                App.DBConnection.AddCase(this.SystemCase, new List<VitalParameters>(this.SystemCase.PreviousVitalParameters));
            }

            if (App.Role == 2)
            {
                this.SystemCase.MedicalId = App.UserID;
                this.SystemCase.MedicalFullName = App.FullNameUser;
            }

            if (!this.NewCase) App.DBConnection.UpdateCase(this.SystemCase);

            this.DialogResult = true;

            Overview.View.Refresh();

            this.Close();
        }
        #endregion

        private void EnableRoleSpecificRegions()
        {
            if (App.Role == 1) // Schwester
            {
                this.regionMedicalData.IsEnabled = false;
                this.regionReleaseData.IsEnabled = false;
                this.btnReleasePatient.IsEnabled = false;
            } else if (App.Role == 2) // Arzt
            {
                this.regionArrivalData.IsEnabled = false;
            }
        }

        private void EnableVitalParameters(bool enable)
        {
            this.btnSaveParameters.IsEnabled = enable;
            this.btnCancelParameters.IsEnabled = enable;

            this.txtHeartFrequence.IsEnabled = enable;
            this.txtBreathFrequence.IsEnabled = enable;
            this.txtBloodPressureMin.IsEnabled = enable;
            this.txtBloodPressureMax.IsEnabled = enable;
            this.txtTemperature.IsEnabled = enable;
            this.txtOxygenSaturation.IsEnabled = enable;
        }

        private void SetVitalParametersFieldsToDefault()
        {
            this.txtHeartFrequence.Text = "";
            this.txtBreathFrequence.Text = "";
            this.txtBloodPressureMin.Text = "";
            this.txtBloodPressureMax.Text = "";
            this.txtTemperature.Text = "";
            this.txtOxygenSaturation.Text = "";
        }

        private bool CheckNecessaryFields()
        {
            if
                (
                this.SystemCase.Priority.Trim().Length != 0 &&
                this.SystemCase.Status.Trim().Length != 0 &&
                this.SystemCase.Location.Trim().Length != 0 &&
                this.SystemCase.Arrival != null &&
                this.SystemCase.Reevaluation != null &&
                this.SystemCase.Complaint.Trim().Length != 0 &&
                this.SystemCase.TypeOfArrival.Trim().Length != 0 &&
                this.SystemCase.Trauma.Trim().Length != 0
                )
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Properties
        public Case SystemCase { get; private set; }
        public Case CaseCopy { get; private set; }
        public bool NewCase { get; private set; } = false;
        #endregion
    }
}