using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using HELP.DataAccess;
using HELP.MainWindows;

namespace HELP.Forms
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string[] credentials;

        public Login()
        {
            credentials = new string[2];

            InitializeComponent();

            var deDE = new System.Globalization.CultureInfo("de-DE", false);
            Thread.CurrentThread.CurrentCulture = deDE;
            Thread.CurrentThread.CurrentUICulture = deDE;
        }

        private void AuthenticateUser()
        {
            if ("".Equals(Username.Text.Trim()))
            {
                Error.Content = "Benutzernamen eingeben!";

                Error.Visibility = Visibility.Visible;

                return;
            }
            else if ("".Equals(Password.Password.ToString().Trim())) {
                Error.Content = "Passwort eingeben!";

                Error.Visibility = Visibility.Visible;

                return;
            }

            Error.Visibility = Visibility.Hidden;
            Progress.Visibility = Visibility.Visible;

            credentials[0] = Username.Text;
            credentials[1] = MD5Hash.HashString(Password.Password.ToString());

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += AuthenticateUser_DoWork;
            worker.RunWorkerCompleted += AuthenticateUser_Completed;

            worker.RunWorkerAsync();

        }

        private void AuthenticateUser_DoWork(object sender, DoWorkEventArgs e) => e.Result = Authentication.Authenticate(credentials);

        private void AuthenticateUser_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Progress.Visibility = Visibility.Hidden;

            if ((int)e.Result > 0)
            {
                (new Overview()).Show();

                Close();
                // How to recognize user role
                /*switch ((int)e.Result)
                {
                    case 1:
                        MessageBox.Show("Nurse");
                        break;
                    case 2:
                        MessageBox.Show("Physician");
                        break;
                    case 3:
                        MessageBox.Show("Admin");
                        break;
                    case 4:
                        MessageBox.Show("Secretary");
                        break;
                }*/

            }
            else if ((int)e.Result == 0)
            {
                Username.Text = "";
                Password.Password = "";
                Error.Content = "Benutzername oder Passwort falsch!";

                Error.Visibility = Visibility.Visible;
            } else if ((int)e.Result == -1)
            {
                MessageBox.Show("Anmeldung für diesen Benutzer gesperrt! Versuchen Sie es später erneut!", "Anmeldung gesperrt");
            }
            else if ((int)e.Result == -2) {
                MessageBox.Show("Cannot connect to application server!", "Error");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == Cancel) Application.Current.Shutdown();
            else AuthenticateUser();
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AuthenticateUser();
        }
    }
}
