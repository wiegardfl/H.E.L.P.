using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.Generic;

using HELP.DataModels;
using HELP.MainWindows;

namespace HELP.Forms
{
    /// <summary>
    /// Interaktionslogik für PatientList.xaml
    /// </summary>
    public partial class PatientList : Window
    {
        private Patient selectedPatient;

        public static List<Patient> patients = new List<Patient>
            {
                new Patient { FullName = "Hans Müller", KVNR = "0123456789", Birthday = new DateTime(2000,1,1), PlaceOfBirth = "Fulda",  Gender = Genders.Maennlich.ToString(), Nationality = "Deutschland", HealthInsurance = "AOK", Address = "Hohenloher Str. 4", PostalCode = "36041", City = "Fulda", Phone = "0661 943783", Mobile = "0177 78989754"},
                new Patient { FullName = "Bernd Müller", KVNR = "0551232133", Birthday = new DateTime(2000,1,1), PlaceOfBirth = "Frankfurt", Gender = Genders.Maennlich.ToString(), Nationality = "Österreich", HealthInsurance = "AOK", Address = "Stauferring 71", PostalCode = "36043", City = "Fulda", Phone = "0661 943783", Mobile = "0177 78989754"},
                new Patient { FullName = "Max Mustermann", KVNR = "9876543210", Birthday = new DateTime(1985,11,7) , PlaceOfBirth = "München", Gender = Genders.Divers.ToString(), Nationality = "Belgien", HealthInsurance = "AOK", Address = "Heinrich Str. 16A", PostalCode = "36042", City = "Fulda", Phone = "0661 943783", Mobile = "0177 78989754"},
                new Patient { FullName = "Fabian Kerz", KVNR = "1122336475", Birthday = new DateTime(2005,3,6), PlaceOfBirth = "Paris",  Gender = Genders.Maennlich.ToString(), Nationality = "Russland", HealthInsurance = "AOK", Address = "Stauferring 71", PostalCode = "36039", City = "Fulda", Phone = "0661 943783", Mobile = "0177 78989754"},
                new Patient { FullName = "Sebastian Schmidt", KVNR = "9524244571", Birthday = new DateTime(1996,9,15), PlaceOfBirth = "Hongkong", Gender = Genders.Weiblich.ToString(), Nationality = "Deutschland", HealthInsurance = "AOK", Address = "Frankfurter Str. 2", PostalCode = "36042", City = "Fulda", Phone = "0661 943783", Mobile = "0177 78989754"}
            };

        public PatientList()
        {
            InitializeComponent();

            PatientsListView.ItemsSource = patients;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(PatientsListView.ItemsSource);

            view.Filter = Filter;
            Loaded += (sender, e) => Keyboard.Focus(Search);
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(PatientsListView.ItemsSource).Refresh();
        }

        private bool Filter(object item)
        {
            if (string.IsNullOrEmpty(Search.Text))
            {
                return true;
            } else
            {
                string[] searchValues = Search.Text.Split(';');

                for (int i = 0; i < searchValues.Length; i++)
                {
                    searchValues[i] = searchValues[i].Trim().ToLower();
                }

                foreach (string searchValue in searchValues)
                {
                    if (
                        !(item as Patient).FullName.ToLower().Contains(searchValue) &&
                        !(item as Patient).KVNR.ToLower().Contains(searchValue) &&
                        !(item as Patient).BirthdayText.Contains(searchValue))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsListView.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie einen Patienten aus!", "Patienten auswählen");
            } else
            {
                //(new CaseWindow((Patient)PatientsListView.SelectedItem)).Show();
                selectedPatient = (Patient)PatientsListView.SelectedItem;

                this.DialogResult = true;

                Close();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            (new Form_Patient()).ShowDialog();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //(new CaseWindow((Patient)PatientsListView.SelectedItem)).Show();
            selectedPatient = (Patient)PatientsListView.SelectedItem;

            this.DialogResult = true;

            Close();
        }

        public Patient GetSelectedPatient { get => selectedPatient; }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (new Form_Patient((Patient)PatientsListView.SelectedItem){ IsEnabled = false }).ShowDialog();
        }
    }
}
