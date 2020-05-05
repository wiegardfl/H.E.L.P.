using System;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;

using DataModels;

namespace HELP.Forms
{
    /// <summary>
    /// Interaktionslogik für PatientList.xaml
    /// </summary>
    public partial class PatientList : Window
    {
        public PatientList()
        {
            InitializeComponent();

            List<Patient> patients = new List<Patient>();

            patients.Add(new Patient { FullName = "Hans Müller", KVNR = "0123456789", Geburtsdatum = "01/01/2000" });
            patients.Add(new Patient { FullName = "Max Mustermann", KVNR = "9876543210", Geburtsdatum = "07/011/1985" });
            patients.Add(new Patient { FullName = "Fabian Kerz", KVNR = "1122336475", Geburtsdatum = "06/03/2005" });

            PatientsListView.ItemsSource = patients;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(PatientsListView.ItemsSource);

            view.Filter = Filter;
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
                if (
                    ((item as Patient).FullName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((item as Patient).KVNR.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((item as Patient).Geburtsdatum.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
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
                MessageBox.Show(((Patient)PatientsListView.SelectedItem).FullName);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            (new Form_Patient()).Show();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show(((Patient)PatientsListView.SelectedItem).FullName);
        }
    }
}
