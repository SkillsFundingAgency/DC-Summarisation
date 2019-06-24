using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Model
{
    public partial class ESF_FundingData
    {
        public int UKPRN { get; set; }
        public string ConRefNumber { get; set; }
        public string DeliverableCode { get; set; }
        public string AttributeName { get; set; }
        public int CollectionYear { get; set; }
        public int CollectionPeriod { get; set; }
        public decimal? Period_1 { get; set; }
        public decimal? Period_2 { get; set; }
        public decimal? Period_3 { get; set; }
        public decimal? Period_4 { get; set; }
        public decimal? Period_5 { get; set; }
        public decimal? Period_6 { get; set; }
        public decimal? Period_7 { get; set; }
        public decimal? Period_8 { get; set; }
        public decimal? Period_9 { get; set; }
        public decimal? Period_10 { get; set; }
        public decimal? Period_11 { get; set; }
        public decimal? Period_12 { get; set; }
    }
}
