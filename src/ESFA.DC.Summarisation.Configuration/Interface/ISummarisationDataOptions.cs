﻿namespace ESFA.DC.Summarisation.Configuration.Interface
{
    public interface ISummarisationDataOptions
    {
        string FCSConnectionString { get; }

        string ILR1819ConnectionString { get; }

        string ILR1920ConnectionString { get; }

        string SummarisedActualsConnectionString { get; }

        string EAS1819ConnectionString { get; }

        string EAS1920ConnectionString { get; }

        string ESFNonEFConnectionString { get; }

        string ESFR2ConnectionString { get; }

        string DASPaymentsConnectionString { get; }

        string ESFFundingDataConnectionString { get; }

        string DataRetrievalMaxConcurrentCalls { get; }
    }
}