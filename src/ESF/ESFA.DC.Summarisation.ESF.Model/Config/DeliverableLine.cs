using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model.Config
{
    public class DeliverableLine
    {
        public string DeliverableCode { get; set; }

        public bool UseAttributes { get; set; }

        public List<string> Attributes { get; set; }

        public bool CalculateVolume { get; set; }
    }
}
