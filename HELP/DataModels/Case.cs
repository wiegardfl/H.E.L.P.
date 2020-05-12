using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HELP.DataModels
{
    public class Case : INotifyPropertyChanged
    {
        private Patient data;
        private static int dailySerialNr = 0;
        private static DateTime currentDay;

        public Case(Patient patient)
        {
            data = patient;

            Arrival = DateTime.Now;
            CurrentDay = Arrival.Date;
            string date = Arrival.ToString("yyMMdd0000");
            CaseNr = long.Parse(date) + ++dailySerialNr;
        }

        #region Properties
        private static DateTime CurrentDay
        {
            get => currentDay;
            set
            {
                if (value != currentDay)
                {
                    currentDay = value;
                    dailySerialNr = 0;
                }
            }
        }

        public DateTime Arrival { get; }

        public string ArrivalFormatted { get => Arrival.ToString("dd.MM.yyyy HH:mm"); }

        public Category Priority { get; set; }

        public long CaseNr { get; }

        public string Name => data?.FullName;

        public Patient Data => data;

        public DateTime Reevaluation { get; set; }

        public string ReevaluationFormatted { get => Reevaluation.ToString("dd.MM.yyyy HH:mm"); }

        public string Status { get; set; }

        public int Age => data.Age;

        public string TreatmentRoom { get; set; }

        public string Complaint { get; set; }

        public string Diagnosis { get; set; }
        #endregion

        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    // Red = Akut, Orange = Sehr Dringed, Yellow = Dringend, Green = Normal, Blue = Nicht Dringend
    public enum Category { Akut = 0, Sehr_Dringend = 10, Dringend = 30, Normal = 90, Nicht_Dringend = 120 }


}