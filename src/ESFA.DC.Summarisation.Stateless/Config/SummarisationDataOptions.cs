using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Stateless.Config
{
    public class SummarisationDataOptions : ISummarisationDataOptions
    {
        public string GenericCollectionsConnectionString { get; set; }

        public string FCSConnectionString { get; set; }

        public string ILR1819ConnectionString { get; set; }

        public string SummarisedActualsConnectionString { get; set; }

        public string EAS1819ConnectionString { get; set; }

        public string DataRetrievalMaxConcurrentCalls { get; set; }

        public string ESFNonEFConnectionString { get; set; }

        public string ESFR2ConnectionString { get; set; }

        public string DASPaymentsConnectionString { get; set; }

        public string ILR1920ConnectionString { get; set; }

        public string ILR2021ConnectionString { get; set; }

        public string EAS1920ConnectionString { get; set; }

        public string EAS2021ConnectionString { get; set; }

        public string ESFFundingDataConnectionString { get; set; }

        public string NcsDbConnectionString { get; set; }

        public string SqlCommandTimeoutSeconds { get; set; }

        public string SummarisedActualsBAUConnectionString { get; set; }
    }
}
