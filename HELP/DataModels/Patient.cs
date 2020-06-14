#region References
using System;
#endregion

namespace HELP.DataModels
{
    public class Patient : ICloneable
    {
        #region Constructors
        public Patient() {}
        #endregion

        #region Methods
        public Patient Clone()
        {
            return (Patient)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region Properties
        public long PatientNr { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName
        {
            get => $"{FirstName} {LastName}";
            set
            {
                if (value.Contains(","))
                {
                    var nachVor = value.Trim().Split(',');
                    LastName = nachVor[0];
                    FirstName = nachVor[1];
                } else
                {
                    var vorNach = value.Trim().Split(' ');
                    FirstName = vorNach[0];
                    if (vorNach.Length > 1) LastName = vorNach[1];
                }
            }
        }
        public string KVNR { get; set; } = "";
        public DateTime Birthday { get; set; } = DateTime.Now;
        public string BirthdayFormatted => Birthday.ToString("dd.MM.yyyy");
        public string PlaceOfBirth { get; set; } = "";
        public int Age => (int)((DateTime.Today - Birthday).Days / 365.25);
        public string Gender { get; set; } = "";
        public string Nationality { get; set; } = "";
        public string HealthInsurance { get; set; } = "";
        public string Address { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string AdditionalInformations { get; set; } = "";
        public string FunctionRelatives { get; set; } = "";
        public string FirstNameRelatives { get; set; } = "";
        public string LastNameRelatives { get; set; } = "";
        public string AddressRelatives { get; set; } = "";
        public string PostalCodeRelatives { get; set; } = "";
        public string CityRelatives { get; set; } = "";
        public string PhoneRelatives { get; set; } = "";
        public string MobileRelatives { get; set; } = "";
        #endregion
    }
}