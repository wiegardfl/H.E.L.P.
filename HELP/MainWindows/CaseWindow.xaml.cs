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

            lblVorname.Content = patient.FullName.Split(' ')[0];
            lblNachname.Content = patient.FullName.Split(' ')[1];
            lblAlter.Content = patient.Alter;
        }
    }
}
