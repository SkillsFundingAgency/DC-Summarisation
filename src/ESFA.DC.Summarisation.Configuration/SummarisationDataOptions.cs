using ESFA.DC.Summarisation.Configuration.Interface;

namespace ESFA.DC.Summarisation.Configuration
{
    public class SummarisationDataOptions : ISummarisationDataOptions
    {
        public string FCSConnectionString { get; set; }

        public string ILR1819ConnectionString { get; set; }

        public string SummarisedActualsConnectionString { get; set; }

        public string EAS1819ConnectionString { get; set; }

        public string DataRetrievalMaxConcurrentCalls { get; set; }

        public string ESFNonEFConnectionString { get; set; }

        public string ESFR2ConnectionString { get; set; }

        public string DASPaymentsConnectionString { get; set; }

        public string ILR1920ConnectionString { get; set; }

        public string EAS1920ConnectionString { get; set; }
    }
}
