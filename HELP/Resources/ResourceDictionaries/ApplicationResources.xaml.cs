using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HELP.Resources.ResourceDictionaries
{
    public partial class ApplicationResources : System.Windows.ResourceDictionary
    {
        public ApplicationResources()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^0-9:. ]").IsMatch(e.Text)) e.Handled = true;
        }

        private void TextBox_PreviewTextInput_Tel(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^0-9 /-]").IsMatch(e.Text)) e.Handled = true;
        }
        private void TextBox_PreviewTextInput_Koerpertemperatur(object sender, TextCompositionEventArgs e)
        {
            if (new System.Text.RegularExpressions.Regex("[^0-9 .,]").IsMatch(e.Text)) e.Handled = true;
        }
    }
}
