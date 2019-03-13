using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class Period : IPeriod
    {
        public int PeriodId { get; set; }

        public decimal? Value { get; set; }
    }
}
