#region References
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using HELP.DataModels;
using HELP.MainWindows;
#endregion

namespace HELP.Resources.UserControls
{
    public partial class CheckBoxFilterMenu : ContextMenu
    {
        #region Constructors
        public CheckBoxFilterMenu(List<string> filters)
        {
            InitializeComponent();

            this.Filters = filters;
        }
        #endregion

        #region Methods

        #region Events
        private void FiltersListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ((ListBox)sender).ItemsSource = this.Filters;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox filter = (CheckBox)sender;

            DynamicData.FilterValues[filter.DataContext.ToString()] = (bool)filter.IsChecked;

            Overview.View.Refresh();
        }
        #endregion

        #endregion

        #region Properties
        public List<string> Filters { get; private set; } = new List<string>();
        #endregion
    }
}
