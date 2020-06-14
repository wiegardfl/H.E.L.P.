﻿#region References
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace HELP.DataModels
{
    public class Case : INotifyPropertyChanged, ICloneable
    {
        #region Variables
        private static int dailySerialNr = 0;
        private static DateTime currentDay;
        #endregion

        #region Constructors
        public Case(Patient patient)
        {
            this.Data = patient;

            CurrentDay = Arrival.Date;

            this.CaseNr = long.Parse(Arrival.ToString("yyMMdd0000")) + ++dailySerialNr;
        }
        #endregion

        #region Methods
        public Case Clone()
        {
            return (Case)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region Properties
        public Patient Data { get; set; } = new Patient();
        public DateTime Arrival { get; private set; } = DateTime.Now;
        public string ArrivalFormatted => Arrival.ToString("dd.MM.yyyy HH:mm");
        public string ArrivalDateFormatted { get => Arrival.ToString("dd.MM.yyyy"); set { } }
        public string ArrivalTimeFormatted { get => Arrival.ToString("HH:mm"); set { } }
        public DateTime Released { get; set; }
        public string TypeOfArrival { get; set; } = "";
        public string PlaceOfIncident { get; set; } = "";
        public string Trauma { get; set; } = "";
        public string OtherInformations { get; set; } = "";
        public string PhysicianLetter { get; set; } = "";
        public string Anamnesis { get; set; } = "";
        public string Services { get; set; } = "";
        public string ExternalServices { get; set; } = "";
        public string Priority { get; set; } = "";
        public int PriorityInt => DynamicData.Priorities[Priority];
        public long CaseNr { get; private set; } = -1;
        public string FirstName => Data?.FirstName;
        public string LastName => Data?.LastName;
        public string Name => Data?.FullName;
        public DateTime Reevaluation { get; set; }
        public string ReevaluationFormatted => Reevaluation.ToString("dd.MM.yyyy HH:mm");
        public string Status { get; set; } = "";
        public int Age => Data.Age;
        public string AgeFormatted => Age > 120 ? "" : Age + " Jahr(e)";
        public string Location { get; set; } = "";
        public string Complaint { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string TypeOfRelease { get; set; } = "";
        public bool AnonymousPatient { get; set; } = false;

        public ObservableCollection<VitalParameters> PreviousVitalParameters { get; set; } = new ObservableCollection<VitalParameters>();

        public ObservableCollection<string> VitalParametersTimes
        {
            get
            {
                ObservableCollection<string> times = new ObservableCollection<string>();

                foreach (VitalParameters vitalParameters in PreviousVitalParameters)
                {
                    times.Add(vitalParameters.Time.ToString("dd.MM.yyyy HH:mm"));
                }

                return times;
            }
            private set { }
        }

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
        #endregion

        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
