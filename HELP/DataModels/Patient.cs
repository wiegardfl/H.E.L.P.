using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELP.DataModels
{
    public class Patient
    {
        public string FullName
        {
            get => $"{FirstName} {LastName}";
            set
            {
                if (value.Contains(','))
                {
                    var nachVor = value.Trim().Split(',');
                    LastName = nachVor[0];
                    FirstName = nachVor[1];
                }
                else
                {
                    var vorNach = value.Trim().Split(' ');
                    FirstName = vorNach[0];
                    if (vorNach.Length > 1) LastName = vorNach[1];
                }
            }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string KVNR { get; set; }
        public string DateOfBirth { get; set; }
        public int Age { get; set; }
    }
}