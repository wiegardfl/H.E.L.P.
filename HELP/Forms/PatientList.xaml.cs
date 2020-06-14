#region References
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using HELP.DataModels;
#endregion

namespace HELP.Forms
{
    public partial class PatientList : Window
    {
        #region Constructors
        public PatientList()
        {
            InitializeComponent();

            ((CollectionView)CollectionViewSource.GetDefaultView(listViewPatients.ItemsSource)).Filter = PatientsFilter;
            Loaded += (sender, e) => Keyboard.Focus(txtSearch);
        }
        #endregion

        #region Methods

        #region Events
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedPatient = (Patient)listViewPatients.SelectedItem;

            this.DialogResult = true;

            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new PatientData((Patient)listViewPatients.SelectedItem) { IsEnabled = false }.ShowDialog();
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listViewPatients.ItemsSource).Refresh();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedPatient = (Patient)listViewPatients.SelectedItem;

            this.DialogResult = true;

            this.Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            PatientData newPatient = new PatientData();

            if (newPatient.ShowDialog() == true)
            {
                SelectedPatient = newPatient.Data;

                this.DialogResult = true;

                this.Close();
            }
        }
        #endregion

        private bool PatientsFilter(object item)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                return true;
            }
            else
            {
                string[] searchValues = txtSearch.Text.Split(';');

                for (int i = 0; i < searchValues.Length; i++)
                {
                    searchValues[i] = searchValues[i].Trim().ToLower();
                }

                foreach (string searchValue in searchValues)
                {
                    if (
                        !(item as Patient).FullName.ToLower().Contains(searchValue) &&
                        !(item as Patient).KVNR.ToLower().Contains(searchValue) &&
                        !(item as Patient).BirthdayFormatted.Contains(searchValue))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion

        #region Properties
        public Patient SelectedPatient { get; private set; } = new Patient();
        #endregion
    }
}
