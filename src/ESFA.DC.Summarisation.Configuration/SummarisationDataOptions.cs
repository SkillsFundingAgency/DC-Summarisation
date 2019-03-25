using ESFA.DC.Summarisation.Configuration.Interface;

namespace ESFA.DC.Summarisation.Configuration
{
    public class SummarisationDataOptions : ISummarisationDataOptions
    {
        public string FCSConnectionString { get; set; }

        public string ILR1819ConnectionString { get; set; }

        public string SummarisedActualsConnectionString { get; set; }
    }
}
