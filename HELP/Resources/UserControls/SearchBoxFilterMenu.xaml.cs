#region References
using System.Windows;
using System.Windows.Controls;

using HELP.DataModels;
using HELP.MainWindows;
#endregion

namespace HELP.Resources.UserControls
{
    public partial class SearchBoxFilterMenu : ContextMenu
    {
        #region Constructors
        public SearchBoxFilterMenu(string header)
        {
            InitializeComponent();

            this.Header = header;
        }
        #endregion

        #region Methods

        #region Events
        private void Header_Loaded(object sender, RoutedEventArgs e)
        {
            ((Label)sender).Content = this.Header + ":";
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            if (this.Header.Equals("Durchgangsnummer")) DynamicData.CaseNrFilter = text;
            else if (this.Header.Equals("Patientenname")) DynamicData.NameFilter = text;

            Overview.View.Refresh();
        }
        #endregion

        #endregion

        #region Properties
        public string Header { get; private set; } = "";
        #endregion
    }
}
