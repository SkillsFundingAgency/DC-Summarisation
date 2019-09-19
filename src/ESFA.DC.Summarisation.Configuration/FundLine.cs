using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Configuration
{
    public class FundLine
    {
        public string Fundline { get; set; }

        public string DeliverableCode { get; set; }

        public string LineType { get; set; }

        public bool UseAttributes { get; set; }

        public List<string> Attributes { get; set; }

        public bool CalculateVolume { get; set; }

        public int? AcademicYear { get; set; }
    }
}
