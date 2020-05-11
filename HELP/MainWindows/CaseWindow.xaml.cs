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

        public CaseWindow(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;
            systemCase = new Case(patient);

            lblVorname.Content = patient.FirstName;
            lblNachname.Content = patient.LastName;
            lblAlter.Content = patient.Age + " Jahr(e)";

            DateTime now = DateTime.Now;
            txtDatumAnkunft.Text = now.ToString("dd.MM.yyyy");
            txtUhrzeitAnkunft.Text = now.ToString("HH:mm");
        }

        public CaseWindow(Case systemCase)
        {
            InitializeComponent();

            this.systemCase = systemCase;
            patient = systemCase.Data;

            lblVorname.Content = patient.FirstName;
            lblNachname.Content = patient.LastName;
            lblAlter.Content = patient.Age + " Jahr(e)";

            txtDatumAnkunft.Text = systemCase.Arrival.ToString("dd.MM.yyyy");
            txtUhrzeitAnkunft.Text = systemCase.Arrival.ToString("HH:mm"); 
        }

        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            //Save changes in DataModel and DataBase
            //

            this.Close();
        }

        private void btnPatientensuche_Click(object sender, RoutedEventArgs e)
        {
            (new PatientList()).Show();

            Close();
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
