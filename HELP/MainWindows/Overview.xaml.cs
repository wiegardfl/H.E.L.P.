using System.Windows;
using System.Collections.Generic;

using HELP.Forms;
using HELP.DataModels;
using System;

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
                new Case(new Patient { FullName = "Hans Müller", KVNR = "0123456789", DateOfBirth = "01/01/2000", Age = 20 })
                { Priority = Category.Red, Reevaluation = DateTime.Now.AddMinutes(5), Status = "In Behandlung", TreatmentRoom = "Zimmer 3", Complaint = "Fieber, Husten", Diagnosis = "COVID-19" },
                new Case(new Patient { FullName = "Bernd Müller", KVNR = "0551232133", DateOfBirth = "01/01/2000", Age = 20 })
                { Priority = Category.Orange, Reevaluation = DateTime.Now.AddMinutes(10), Status = "In Behandlung", TreatmentRoom = "Zimmer 1", Complaint = "Schmerzen im Bein", Diagnosis = "Fibulafraktur" },
                new Case(new Patient { FullName = "Max Mustermann", KVNR = "9876543210", DateOfBirth = "07/11/1985", Age = 34 })
                { Priority = Category.Yellow, Reevaluation = DateTime.Now.AddMinutes(15), Status = "Im Wartezimmer", TreatmentRoom = "Im Wartezimmer", Complaint = "Kopfschmerzen, Übelkeit", Diagnosis = "Magen-Darm Infektion" },
                new Case(new Patient { FullName = "Fabian Kerz", KVNR = "1122336475", DateOfBirth = "06/03/2005", Age = 15 })
                { Priority = Category.Green, Reevaluation = DateTime.Now.AddMinutes(20), Status = "In Beobachtung", TreatmentRoom = "Im Wartezimmer", Complaint = "Daumen blutet", Diagnosis = "Schnittwunde" },
                new Case(new Patient { FullName = "Sebastian Schmidt", KVNR = "9524244571", DateOfBirth = "15/09/1996", Age = 23})
                { Priority = Category.Blue, Reevaluation = DateTime.Now.AddMinutes(25), Status = "Im Wartezimmer", TreatmentRoom = "Im Wartezimmer", Complaint = "Nase läuft", Diagnosis = "Grippaler Infekt" }
            };

            ListViewMain.ItemsSource = cases;
        }

        private void NewCaseBtn_Click(object sender, RoutedEventArgs e)
        {
            (new PatientList()).Show();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (new CaseWindow((Case)ListViewMain.SelectedItem)).Show();
        }
    }
}
