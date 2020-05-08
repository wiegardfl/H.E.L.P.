using System;
using System.Windows;

using HELP.DataModels;


namespace HELP.MainWindows
{
    /// <summary>
    /// Interaktionslogik für CaseWindow.xaml
    /// </summary>
    public partial class CaseWindow : Window
    {
        private Patient patient;

        public CaseWindow(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;

            lblVorname.Content = patient.Vorname;
            lblNachname.Content = patient.Nachname;
            lblAlter.Content = patient.Alter;

            DateTime now = DateTime.Now;
            txtDatumAnkunft.Text = now.ToString("dd.MM.yyyy");
            txtUhrzeitAnkunft.Text = now.ToString("HH:mm");
        }

        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            //Save changes in DataModel and DataBase
            //

            this.Close();
        }
    }
}
