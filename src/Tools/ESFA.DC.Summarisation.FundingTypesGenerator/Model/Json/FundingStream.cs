using System.Collections.Generic;

namespace FundingTypesGenerator.Model.Json
{
    public class FundingStream
    {
        public string PeriodCode { get; set; }
        public string FundModel { get; set; }
        public int DeliverableLineCode { get; set; }
        public List<FundLine> FundLines { get; set; }
    }
}