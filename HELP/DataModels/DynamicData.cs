#region References
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace HELP.DataModels
{
    sealed class DynamicData
    {
        #region Constructors
        private DynamicData() {}
        #endregion

        #region Properties
        public static Dictionary<string, int> Priorities { get; private set; } = new Dictionary<string, int>();
        public static List<string> Statuses { get; private set; } = new List<string>();
        public static List<string> Locations { get; private set; } = new List<string>();
        public static List<string> Genders { get; private set; } = new List<string>() { "Männlich", "Weiblich", "Divers" };
        public static List<string> Nationalities { get; private set; } = new List<string>();
        public static List<string> HealthInsurances { get; private set; } = new List<string>();
        public static List<string> PrioritiesAsList => new List<string>(Priorities.Keys);
        public static List<string> ReleaseTypes { get; private set; } = new List<string>() { "Entlassung nach Hause", "Krankenhausaufenthalt", "Verlegung in ein anderes Krankenhaus", "Freiwilliger Austritt", "Verstorben" };
        public static List<string> FunctionsRelatives { get; private set; } = new List<string>() { "Mutter", "Vater", "Großvater", "Großmutter", "Bruder", "Schwester", "Lebenspartner", "Enkelkind" };
        public static ObservableCollection<Patient> Patients { get; private set; } = new ObservableCollection<Patient>();
        public static ObservableCollection<Case> Cases { get; private set; } = new ObservableCollection<Case>();

        #region OverviewFilters
        public static Dictionary<string, bool> FilterValues { get; private set; } = new Dictionary<string, bool>();
        public static string NameFilter { get; set; } = "";
        public static string CaseNrFilter { get; set; } = "";
        #endregion

        #endregion
    }
}
