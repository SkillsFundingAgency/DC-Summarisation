﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.output.Model
{
    public class SummarisedActual
    {
        public string OrganisationId { get; set; }
        public string UoPcode { get; set; }
        public string FundingStreamPeriodCode { get; set; }
        public int Period { get; set; }
        public int DeliverableCode { get; set; }
        public int ActualVolume { get; set; }
        public decimal ActualValue { get; set; }
        public string PeriodTypeCode { get; set; }
        public string ContractAllocationNumber { get; set; }
    }
}