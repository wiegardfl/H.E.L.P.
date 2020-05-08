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
            Loaded += (sender, e) => Keyboard.Focus(txtVorname);
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            new MainWindows.CaseWindow(new DataModels.Patient() { }).Show();
            this.Close();
        }
    }
}
