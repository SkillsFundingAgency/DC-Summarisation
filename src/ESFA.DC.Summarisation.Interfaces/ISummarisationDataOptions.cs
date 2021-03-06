﻿namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationDataOptions
    {
        string GenericCollectionsConnectionString { get; }

        string FCSConnectionString { get; }

        string ILR1819ConnectionString { get; }

        string ILR1920ConnectionString { get; }

        string ILR2021ConnectionString { get; }

        string SummarisedActualsConnectionString { get; }

        string EAS1819ConnectionString { get; }

        string EAS1920ConnectionString { get; }

        string EAS2021ConnectionString { get; }

        string ESFNonEFConnectionString { get; }

        string ESFR2ConnectionString { get; }

        string DASPaymentsConnectionString { get; }

        string ESFFundingDataConnectionString { get; }

        string DataRetrievalMaxConcurrentCalls { get; }

        string SqlCommandTimeoutSeconds { get; }

        string NcsDbConnectionString { get; }

        string SummarisedActualsBAUConnectionString { get; }
    }
}