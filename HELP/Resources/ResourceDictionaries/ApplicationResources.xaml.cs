#region References
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
#endregion

namespace HELP.Resources.ResourceDictionaries
{
    public partial class ApplicationResources : ResourceDictionary
    {
        #region Constructors
        public ApplicationResources()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods

        #region Events
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (new Regex("[^0-9:. ]").IsMatch(e.Text)) e.Handled = true;
        }

        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^0-9 /-]").IsMatch(e.Text)) e.Handled = true;
        }

        private void LettersBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^A-Z a-z]").IsMatch(e.Text)) e.Handled = true;
        }

        private void LettersAndNumbersBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^A-Z a-z0-9]").IsMatch(e.Text)) e.Handled = true;
        }

        private void NumbersAndSignsBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^0-9 .,]").IsMatch(e.Text)) e.Handled = true;
        }
        #endregion

        #endregion
    }
}
