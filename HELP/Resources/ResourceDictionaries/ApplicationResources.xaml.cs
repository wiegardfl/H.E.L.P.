﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
