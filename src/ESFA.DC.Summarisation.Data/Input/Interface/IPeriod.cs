namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriod
    {
        int PeriodId { get; }

        int CalendarMonth { get; }

        int CalendarYear { get; }

        int CollectionMonth { get; }

        int CollectionYear { get; }

        decimal? Value { get; }

        int? Volume { get; }

    }
}
