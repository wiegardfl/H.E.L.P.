using HELP.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HELP.Forms
{
    /// <summary>
    /// Interaktionslogik für Form_Patient.xaml
    /// </summary>
    public partial class Form_Patient : Window
    {
        public Form_Patient()
        {
            InitializeComponent();
            this.DataContext = PatientData;
            Loaded += (sender, e) => Keyboard.Focus(txtVorname);
        }

        public Form_Patient(Patient patient) : this()
        {
            PatientData = patient;
            DataContext = PatientData;

            //MessageBox.Show(patient.Gender.ToString().Replace("ae", "ä"));
            var text = patient.Gender.ToString();
            cmbGeschlecht.SelectedValue = text.Replace("ae", "ä");
        }

        public Patient PatientData { get; private set; } = new Patient();
        public Patient Copy { get; private set; } = new Patient();
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            //new MainWindows.CaseWindow(new DataModels.Patient() { }).Show();
            this.Close();
        }

        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
