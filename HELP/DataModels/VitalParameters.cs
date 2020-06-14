using System;

namespace HELP.DataModels
{
    public class VitalParameters
    {
        #region Constructors
        public VitalParameters() {}
        #endregion

        #region Properties
        public DateTime Time { get; set; } = DateTime.Now;
        public int? HeartFrequence { get; set; }
        public int? BreathFrequence { get; set; }
        public int? BloodPressureMin { get; set; }
        public int? BloodPressureMax { get; set; }
        public double? Temperature { get; set; }
        public int? OxygenSaturation { get; set; }
        #endregion
    }
}