using System.Windows;
using System.Collections.Generic;

using HELP.Forms;
using HELP.DataModels;
using System;
using Renci.SshNet.Messages;

namespace HELP.MainWindows
{
    /// <summary>
    /// Interaktionslogik für Overview.xaml
    /// </summary>
    public partial class Overview : Window
    {
        public Overview()
        {
            InitializeComponent();

            List<Case> cases = new List<Case>
            {
                new Case(PatientList.patients[0])
                { Priority = Category.Akut, Reevaluation = DateTime.Now.AddMinutes(5), Status = "In Behandlung", TreatmentRoom = "Zimmer 3", Complaint = "Fieber, Husten", Diagnosis = "COVID-19" },
                new Case(PatientList.patients[1])
                { Priority = Category.Sehr_Dringend, Reevaluation = DateTime.Now.AddMinutes(10), Status = "In Behandlung", TreatmentRoom = "Zimmer 1", Complaint = "Schmerzen im Bein", Diagnosis = "Fibulafraktur" },
                new Case(PatientList.patients[2])
                { Priority = Category.Dringend, Reevaluation = DateTime.Now.AddMinutes(15), Status = "Im Wartezimmer", TreatmentRoom = "Im Wartezimmer", Complaint = "Kopfschmerzen, Übelkeit", Diagnosis = "Magen-Darm Infektion" },
                new Case(PatientList.patients[3])
                { Priority = Category.Normal, Reevaluation = DateTime.Now.AddMinutes(20), Status = "In Beobachtung", TreatmentRoom = "Im Wartezimmer", Complaint = "Daumen blutet", Diagnosis = "Schnittwunde" },
                new Case(PatientList.patients[4])
                { Priority = Category.Nicht_Dringend, Reevaluation = DateTime.Now.AddMinutes(25), Status = "Im Wartezimmer", TreatmentRoom = "Im Wartezimmer", Complaint = "Nase läuft", Diagnosis = "Grippaler Infekt" }
            };

            ListViewMain.ItemsSource = cases;
        }

        private void NewCaseBtn_Click(object sender, RoutedEventArgs e)
        {
            (new CaseWindow()).ShowDialog();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (new CaseWindow((Case)ListViewMain.SelectedItem)).ShowDialog();
        }
    }
}
