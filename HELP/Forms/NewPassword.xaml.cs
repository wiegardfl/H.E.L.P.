#region References
using System.Windows;
#endregion

namespace HELP.Forms
{
    public partial class NewPassword : Window
    {
        #region Variables
        private int userID;
        #endregion

        #region Constructors
        public NewPassword(int userID)
        {
            InitializeComponent();

            this.userID = userID;
        }
        #endregion

        #region Methods

        #region Events
        public void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtNewPassword.Password.Equals(this.txtRepeatPassword.Password))
            {
                App.DBConnection.SetUserPassword(userID, this.txtNewPassword.Password);

                this.DialogResult = true;

                this.Close();
            } else
            {
                MessageBox.Show("Passwörter stimmen nicht überein!", "Passwörter stimmen nicht überein");
            }
        }

        #endregion

        #endregion
    }
}
