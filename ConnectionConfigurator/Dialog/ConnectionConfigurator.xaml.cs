#region References
using System.Collections.Generic;
using System.Windows;

using ConnectionConfigurator.DataAccess;
#endregion

namespace ConnectionConfigurator.Dialog
{
    public partial class ConnectionConfigurator : Window
    {
        #region Constructors
        public ConnectionConfigurator()
        {
            InitializeComponent();

            Dictionary<string, string> values = ConfigFile.ReadConfig();

            try
            {
                txtServer.Text = values["server"];
                txtPort.Text = values["port"];
                txtDatabase.Text = values["database"];
                txtUsername.Text = values["username"];
                txtPassword.Password = values["password"];
            } catch (KeyNotFoundException e)
            {
                txtServer.Text = "";
                txtPort.Text = "";
                txtDatabase.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

                ConfigFile.RecreateCorruptConfig();

                MessageBox.Show("Config was corrupted! Recreated it.", "Config corrupted");
            }
            
        }
        #endregion

        #region Methods

        #region Events
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string[] values = { txtServer.Text, txtPort.Text, txtDatabase.Text, txtUsername.Text, txtPassword.Password };

            ConfigFile.WriteConfig(values);

            this.Close();
        }
        #endregion

        #endregion
    }
}