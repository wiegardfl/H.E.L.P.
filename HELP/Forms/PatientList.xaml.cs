﻿using System;
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

        public PatientList()
        {
            InitializeComponent();

            List<Patient> patients = new List<Patient>
            {
                new Patient { FullName = "Hans Müller", KVNR = "0123456789", DateOfBirth = "01.01.2000", Age = 20, Gender = Genders.Maennlich.ToString(), Nationality = "Deutschland", HealthInsurance = "AOK", Address = "Hohenloher Str. 4", PostalCode = "36041", City = "Fulda", Phone = "0661943783", Mobile = "017778989754"},
                new Patient { FullName = "Bernd Müller", KVNR = "0551232133", DateOfBirth = "01.01.2000", Age = 20, Gender = Genders.Maennlich.ToString(), Nationality = "Österreich", HealthInsurance = "AOK", Address = "Stauferring 71", PostalCode = "36043", City = "Fulda", Phone = "0661943783", Mobile = "017778989754"},
                new Patient { FullName = "Max Mustermann", KVNR = "9876543210", DateOfBirth = "07.11.1985", Age = 34, Gender = Genders.Maennlich.ToString(), Nationality = "Belgien", HealthInsurance = "AOK", Address = "Heinrich Str. 16A", PostalCode = "36042", City = "Fulda", Phone = "0661943783", Mobile = "017778989754"},
                new Patient { FullName = "Fabian Kerz", KVNR = "1122336475", DateOfBirth = "06.03.2005", Age = 15, Gender = Genders.Maennlich.ToString(), Nationality = "Russland", HealthInsurance = "AOK", Address = "Stauferring 71", PostalCode = "36039", City = "Fulda", Phone = "0661943783", Mobile = "017778989754"},
                new Patient { FullName = "Sebastian Schmidt", KVNR = "9524244571", DateOfBirth = "15.09.1996", Age = 23, Gender = Genders.Maennlich.ToString(), Nationality = "Deutschland", HealthInsurance = "AOK", Address = "Frankfurter Str. 2", PostalCode = "36042", City = "Fulda", Phone = "0661943783", Mobile = "017778989754"}
            };

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
                List<string> searchValues = new List<string>(Search.Text.Split(';'));

                for (int i = 0; i < searchValues.Count; i++)
                {
                    searchValues[i] = searchValues[i].Trim();
                }


                // Alternative Lösung villeicht mit Rekursion?
                if (
                    ((item as Patient).FullName.IndexOf(searchValues[0], StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((item as Patient).KVNR.IndexOf(searchValues[0], StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((item as Patient).DateOfBirth.IndexOf(searchValues[0], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (searchValues.Count > 1)
                    {
                        if (
                            ((item as Patient).FullName.IndexOf(searchValues[1], StringComparison.OrdinalIgnoreCase) >= 0) ||
                            ((item as Patient).KVNR.IndexOf(searchValues[1], StringComparison.OrdinalIgnoreCase) >= 0) ||
                            ((item as Patient).DateOfBirth.IndexOf(searchValues[1], StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            if (searchValues.Count > 2)
                            {
                                if (
                                    ((item as Patient).FullName.IndexOf(searchValues[2], StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    ((item as Patient).KVNR.IndexOf(searchValues[2], StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    ((item as Patient).DateOfBirth.IndexOf(searchValues[2], StringComparison.OrdinalIgnoreCase) >= 0))
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
                    } else
                    {
                        return true;
                    }
                } else
                {
                    return false;
                }
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
            (new Form_Patient()).Show();
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
            (new Form_Patient((Patient)PatientsListView.SelectedItem)).ShowDialog();
        }
    }
}
