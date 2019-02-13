﻿using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Model
{
    public partial class SummarisedActual
    {
        public int Id { get; set; }
        public int CollectionReturnId { get; set; }
        public string OrganisationId { get; set; }
        public string UoPcode { get; set; }
        public string FundingStreamPeriodCode { get; set; }
        public int Period { get; set; }
        public int DeliverableCode { get; set; }
        public int ActualVolume { get; set; }
        public decimal ActualValue { get; set; }
        public string PeriodTypeCode { get; set; }
        public string ContractAllocationNumber { get; set; }

        public virtual CollectionReturn CollectionReturn { get; set; }
    }
}
