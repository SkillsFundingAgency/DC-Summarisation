namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriod
    {
        int PeriodId { get; }

        decimal? Value { get; }

        int? Volume { get; }
    }
}
