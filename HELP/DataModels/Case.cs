#region References
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
        #region Constructors
        public Case(Patient patient)
        {
            this.Data = patient;
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
        public DateTime Arrival { get; set; } = DateTime.Now;
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
        public long CaseNr { get; set; } = 0;
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
        public int MedicalId { get; set; } = 0;
        public string MedicalFullName { get; set; } = "";
        public int NurseId { get; set; } = 0;
        public string NurseFullName { get; set; } = "";
        public string CaseStatus { get; set; } = "Open";

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
