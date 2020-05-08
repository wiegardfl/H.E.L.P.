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
            get => $"{Vorname} {Nachname}";
            set
            {
                if (value.Contains(','))
                {
                    var nachVor = value.Trim().Split(',');
                    Nachname = nachVor[0];
                    Vorname = nachVor[1];
                }
                else
                {
                    var vorNach = value.Trim().Split(' ');
                    Vorname = vorNach[0];
                    if (vorNach.Length > 1) Nachname = vorNach[1];
                }
            }
        }

        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string KVNR { get; set; }
        public string Geburtsdatum { get; set; }
        public int Alter { get; set; }
    }
}