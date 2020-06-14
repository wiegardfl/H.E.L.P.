#region References
using System.Windows;
using System.Windows.Input;

using HELP.MainWindows;
#endregion

namespace HELP.Forms
{
    public partial class Login : Window
    {
        #region Constructors
        public Login()
        {
            InitializeComponent();

            Loaded += (sender, e) => this.txtUsername.Focus();
        }
        #endregion

        #region Methods

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.btnCancel) this.Close();
            else dummyLogin();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) dummyLogin();
        }
        #endregion

        private void dummyLogin()
        {
            if (this.txtUsername.Text.Equals("Schwester") && this.txtPassword.Password.Equals("password")) App.Role = 1;
            else if (this.txtUsername.Text.Equals("Arzt") && this.txtPassword.Password.Equals("password")) App.Role = 2;
            else return;

            new Overview().Show();

            this.Close();
        }

        #endregion
    }
}
