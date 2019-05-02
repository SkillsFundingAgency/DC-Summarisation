namespace ESFA.DC.Summarisation.Configuration.Interface
{
    public interface ISummarisationDataOptions
    {
        string FCSConnectionString { get; }

        string ILR1819ConnectionString { get; }

        string SummarisedActualsConnectionString { get; }

        string EAS1819ConnectionString { get; }

        string DataRetrievalMaxConcurrentCalls { get; }
    }
}