using System;
using System.Windows;

using HELP.DataModels;
using HELP.Forms;


namespace HELP.MainWindows
{
    /// <summary>
    /// Interaktionslogik für CaseWindow.xaml
    /// </summary>
    public partial class CaseWindow : Window
    {
        private Patient patient;
        private Case systemCase;

        public CaseWindow()
        {
            InitializeComponent();

            this.DataContext = CaseData;

            // this.patient = patient;
            // systemCase = new Case(patient);

            //lblVorname.Content = patient.FirstName;
            //lblNachname.Content = patient.LastName;
            //lblAlter.Content = patient.Age + " Jahr(e)";

            //DateTime now = DateTime.Now;
            //txtDatumAnkunft.Text = now.ToString("dd.MM.yyyy");
            //txtUhrzeitAnkunft.Text = now.ToString("HH:mm");
        }

        public CaseWindow(Case systemCase)
        {
            InitializeComponent();

            this.systemCase = systemCase;
            patient = systemCase.Data;

            this.DataContext = systemCase;

            //lblVorname.Content = patient.FirstName;
            //lblNachname.Content = patient.LastName;
            //lblAlter.Content = patient.Age + " Jahr(e)";

            cmbPrioritaet.SelectedValue = Convert.ToString(systemCase.Priority).Replace('_', ' ');

            //txtDatumAnkunft.Text = systemCase.Arrival.ToString("dd.MM.yyyy");
            //txtUhrzeitAnkunft.Text = systemCase.Arrival.ToString("HH:mm"); 
        }

        public Case CaseData { get; private set; } = new Case(new Patient());
        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            //Save changes in DataModel and DataBase
            //

            this.Close();
        }

        private void btnPatientensuche_Click(object sender, RoutedEventArgs e)
        {
            PatientList patientList = new PatientList();

            if (patientList.ShowDialog() == true)
            {
                patient = patientList.GetSelectedPatient;

                //CaseData.Data = patient;
                systemCase = new Case(patient);

                lblVorname.Content = patient.FirstName;
                lblNachname.Content = patient.LastName;
                lblAlter.Content = patient.Age + " Jahr(e)";
            }
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDatenBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            (new Form_Patient(systemCase.Data)).ShowDialog();
        }
    }
}
