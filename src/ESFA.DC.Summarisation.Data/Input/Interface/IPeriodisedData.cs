namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriodisedData
    {
        int UKPRN { get; }

        string LearnRefNumber { get; }

        string AttributeName { get; }

        decimal? Period1 { get; }

        decimal? Period2 { get; }

        decimal? Period3 { get; }

        decimal? Period4 { get; }

         decimal? Period5 { get; }

        decimal? Period6 { get; }

        decimal? Period7 { get; }

        decimal? Period8 { get; }

        decimal? Period9 { get; }

        decimal? Period10 { get; }

        decimal? Period11 { get; }

        decimal? Period12 { get; }
    }
}
