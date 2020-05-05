using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELP.DataModels
{
    public class Patient
    {
        public string FullName { get; set; }
        public string KVNR { get; set; }
        public string Geburtsdatum { get; set; }
        public int Alter { get; set; }
    }
}