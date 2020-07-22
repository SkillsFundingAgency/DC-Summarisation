using System.Collections.Generic;

namespace FundingTypesGenerator.Model.SpreadSheet
{
    public class SSDeliverableLineCode
    {
        public int LineCode { get; set; }
        public string DeliverableName { get; set; }
        public string FundingType { get; set; }
        public List<SSFundLine> FundLines { get; set; }
    }
}