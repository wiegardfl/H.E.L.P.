using System.Windows;

using HELP.Forms;


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
        }

        private void NewCaseBtn_Click(object sender, RoutedEventArgs e)
        {
            (new PatientList()).Show();
        }
    }
}
