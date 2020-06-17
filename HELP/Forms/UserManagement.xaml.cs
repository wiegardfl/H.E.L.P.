#region References
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using HELP.DataModels;
#endregion

namespace HELP.Forms
{
    public partial class UserManagement : Window
    {
        #region Constructors
        public UserManagement()
        {
            InitializeComponent();

            LoadUsers();

            Closing += (sender, e) =>
            {
                if (App.CloseState == 0) Application.Current.Shutdown();
            };

            Closed += (sender, e) => App.CloseState = 0;
        }
        #endregion

        #region Methods

        #region Events
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CloseState = 1;
            App.Role = 0;
            App.FullNameUser = "";

            DynamicData.Users.Clear();

            new Login(false).Show();

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> usernames = new List<string>();

            foreach (User user in DynamicData.Users)
            {
                usernames.Add(user.UserName);
            }

            if (this.usersListView.SelectedItem != null)
            {
                for (int i = 0; i < DynamicData.Users.Count; i++)
                {
                    if (DynamicData.Users[i].ID == ((User)this.usersListView.SelectedItem).ID)
                    {
                        usernames.Remove(((User)this.usersListView.SelectedItem).UserName);

                        if (usernames.Contains(this.txtUserName.Text))
                        {
                            MessageBox.Show("Ein Benutzer mit diesem Benutzernamen existiert bereits", "Benutzername existiert bereits");

                            return;
                        }

                        DynamicData.Users[i].FirstName = this.txtFirstName.Text;
                        DynamicData.Users[i].LastName = this.txtLastName.Text;
                        DynamicData.Users[i].UserName = this.txtUserName.Text;
                        DynamicData.Users[i].Role = ((ComboBoxItem)this.cmbRole.SelectedItem).Content.ToString();

                        CollectionViewSource.GetDefaultView(this.usersListView.ItemsSource).Refresh();

                        App.DBConnection.UpdateUser(DynamicData.Users[i], (bool)this.checkResetPassword.IsChecked);

                        ClearSelection();

                        break;
                    }
                }
            } else
            {
                if (usernames.Contains(this.txtUserName.Text))
                {
                    MessageBox.Show("Ein Benutzer mit diesem Benutzernamen existiert bereits", "Benutzername existiert bereits");

                    return;
                }

                User user = new User() { FirstName = this.txtFirstName.Text, LastName = this.txtLastName.Text, UserName = this.txtUserName.Text, Role = ((ComboBoxItem)this.cmbRole.SelectedItem).Content.ToString() };

                DynamicData.Users.Add(user);

                CollectionViewSource.GetDefaultView(this.usersListView.ItemsSource).Refresh();

                App.DBConnection.AddUser(user);

                ClearSelection();
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (this.usersListView.SelectedItem == null)
            {
                MessageBox.Show("Kein Benutzer ausgewählt!", "Kein Benutzer ausgewählt");

                return;
            }

            User user = (User)this.usersListView.SelectedItem;

            DynamicData.Users.Remove(user);

            CollectionViewSource.GetDefaultView(this.usersListView.ItemsSource).Refresh();

            App.DBConnection.RemoveUser(user);
        }

        private void CheckResetPassword_Changed(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true) this.lblInfo.Visibility = Visibility.Visible;
            else this.lblInfo.Visibility = Visibility.Hidden;
        }

        private void ListViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            User item = (User)((ListViewItem)sender).Content;

            this.txtFirstName.Text = item.FirstName;
            this.txtLastName.Text = item.LastName;
            this.txtUserName.Text = item.UserName;
            this.cmbRole.SelectedValue = item.Role;
        }
        #endregion

        private void LoadUsers()
        {
            App.DBConnection.GetUsers();

            this.usersListView.ItemsSource = DynamicData.Users;
        }

        private void ClearSelection()
        {
            this.usersListView.SelectedItem = null;

            this.txtFirstName.Text = "";
            this.txtLastName.Text = "";
            this.txtUserName.Text = "";
            this.cmbRole.SelectedIndex = -1;
            this.checkResetPassword.IsChecked = false;
        }
        #endregion
    }
}