namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class Period
    {
        public int PeriodId { get; set; }

        public int CalendarMonth { get; set; }

        public int CalendarYear { get; set; }

        public int CollectionMonth { get; set; }

        public int CollectionYear { get; set; }

        public decimal? Value { get; set; }

        public int? Volume { get; set; }
    }
}
