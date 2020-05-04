using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    class Case : INotifyPropertyChanged
    {
        private Patient data;
        private static int dailySerialNr = 0;
        private static DateTime currentDay;

        public Case()
        {
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

        public Category Priority { get; set; }

        public long CaseNr { get; }

        public string Name => data?.FullName;
        #endregion

        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public enum Category { Red = 0, Orange = 10, Yellow = 30, Green = 90, Blue = 120 }
}