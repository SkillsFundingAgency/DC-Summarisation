using System.Collections.Generic;

namespace FundingTypesGenerator.Model.SpreadSheet
{
    public class SSFundingStream
    {
        public string PeriodCode { get; set; }
        public List<SSDeliverableLineCode> DeliverableLineCodes { get; set; }

    }
}