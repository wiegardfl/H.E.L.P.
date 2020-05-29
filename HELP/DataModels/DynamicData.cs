using System.Collections.Generic;

namespace HELP.DataModels
{
    sealed class DynamicData
    {
        public static Dictionary<string, int> priorities = new Dictionary<string, int>();
        public static List<string> statuses = new List<string>();
        public static List<string> treatmentRooms = new List<string>();

        private DynamicData() { }

        #region Properties
        public static List<string> PrioritiesAsList => new List<string>(priorities.Keys);
        #endregion
    }
}
