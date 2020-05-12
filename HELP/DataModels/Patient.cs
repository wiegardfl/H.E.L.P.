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
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string HealthInsurance { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string AdditionalInformation { get; set; }
        public string FirstNameRelatives { get; set; }
        public string LastNameRelatives { get; set; }
        public string AddressRelatives { get; set; }
        public string PostalCodeRelatives { get; set; }
        public string CityRelatives { get; set; }
        public string PhoneRelatives { get; set; }
        public string MobileRelatives { get; set; }
    }

    public enum Genders { Maennlich = 0, Weiblich = 1, Divers = 2 }
}