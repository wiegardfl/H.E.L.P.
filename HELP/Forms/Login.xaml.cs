#region References
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using HELP.DataAccess;
using HELP.MainWindows;
#endregion

namespace HELP.Forms
{
    public partial class Login : Window
    {
        #region Variables
        private LoadingWindow loadingWindow;
        private string[] credentials;
        #endregion

        #region Constructors
        public Login() : this(true) {}

        public Login(bool init)
        {
            InitializeComponent();

            this.loadingWindow = new LoadingWindow();
            this.credentials = new string[2];

            if (init)
            {
                BackgroundWorker loadingWorker = new BackgroundWorker();

                loadingWorker.DoWork += Loading_DoWork;
                loadingWorker.RunWorkerCompleted += Loading_Completed;

                this.loadingWindow.Show();

                loadingWorker.RunWorkerAsync();
            }
            else this.Visibility = Visibility.Visible;

            Loaded += (sender, e) => this.txtUsername.Focus();

            Closing += (sender, e) =>
            {
                if (App.CloseState == 0) Application.Current.Shutdown();
            };

            Closed += (sender, e) => App.CloseState = 0;
        }
        #endregion

        #region Methods

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.btnCancel) this.Close();
            else Authenticate();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Authenticate();
        }
        #endregion

        private void Authenticate()
        {
            if ("".Equals(this.txtUsername.Text.Trim()))
            {
                this.lblError.Content = "Benutzernamen eingeben!";
                this.lblError.Visibility = Visibility.Visible;

                return;
            } else if ("".Equals(this.txtPassword.Password.Trim()))
            {
                this.lblError.Content = "Passwort eingeben!";
                this.lblError.Visibility = Visibility.Visible;

                return;
            }

            lblError.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Visible;

            credentials[0] = this.txtUsername.Text;
            credentials[1] = MD5Hash.HashString(this.txtPassword.Password);

            BackgroundWorker authenticationWorker = new BackgroundWorker();

            authenticationWorker.DoWork += Authenticate_DoWork;
            authenticationWorker.RunWorkerCompleted += Authenticate_Completed;

            authenticationWorker.RunWorkerAsync();
        }

        private void Authenticate_DoWork(object sender, DoWorkEventArgs e) => e.Result = App.DBConnection.Authenticate(credentials);

        private void Authenticate_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            int result = (int)e.Result;

            this.progressBar.Visibility = Visibility.Hidden;

            if (result > 0)
            {
                App.CloseState = 1;

                new Overview().Show();

                this.Close();
            } else if (result == 0)
            {
                this.txtUsername.Text = "";
                this.txtPassword.Password = "";
                this.lblError.Content = "Benutzername oder Passwort falsch!";
                this.lblError.Visibility = Visibility.Visible;
            } else if (result == -1)
            {
                MessageBox.Show("Anmeldung für diesen Benutzer gesperrt! Versuchen Sie es später erneut!", "Anmeldung gesperrt");
            } else if (result == -2)
            {
                MessageBox.Show("Verbindung zum Anwendungsserver fehlgeschlagen!", "Verbindung fehlgeschlagen");
            }
        }

        private void Loading_DoWork(object sender, DoWorkEventArgs e) => e.Result = App.DBConnection.InitConnection();

        private void Loading_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            int result = (int)e.Result;

            this.loadingWindow.Close();

            switch (result)
            {
                case 0:
                    this.Visibility = Visibility.Visible;

                    break;
                case 1:
                    MessageBox.Show("Es konnte keine Konfigurationsdatei gefunden werden!\nBitte verwenden Sie den \"ConnectionConfigurator\"!", "Keine Konfigurationsdatei");

                    this.Close();

                    break;
                case 2:
                    MessageBox.Show("Konfigurationsdatei ist ungültig!\nBitte verwenden Sie den \"ConnectionConfigurator\"!", "Ungültige Konfigurationsdatei");

                    this.Close();

                    break;
                case 3:
                    MessageBox.Show("Es konnte keine Verbindung zum Anwendungsserver hergestellt werden!", "Verbindung fehlgeschlagen");

                    this.Close();

                    break;
                case 4:
                    MessageBox.Show("Unbekannter Fehler!", "Unbekannter Fehler");

                    this.Close();

                    break;
            }
        }

        #endregion
    }
}
