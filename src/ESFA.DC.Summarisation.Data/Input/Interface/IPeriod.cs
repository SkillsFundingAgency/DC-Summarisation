namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriod
    {
        int PeriodId { get; }

        int CalendarMonth { get; }

        int CalendarYear { get; }

        decimal? Value { get; }

        int? Volume { get; }
    }
}
