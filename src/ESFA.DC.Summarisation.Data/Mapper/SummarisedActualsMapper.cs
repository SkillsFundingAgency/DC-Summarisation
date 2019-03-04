using ESFA.DC.Summarisation.Data.Mapper.Interface;
using ESFA.DC.Summarisation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Mapper
{
    public class SummarisedActualsMapper : ISummarisedActualsMapper
    {
        public IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<output.Model.SummarisedActual> actuals)
        {
            return actuals.Select(actual => new SummarisedActual
            {
                OrganisationId = actual.OrganisationId,
                UoPcode = actual.UoPcode,
                FundingStreamPeriodCode = actual.FundingStreamPeriodCode,
                Period = actual.Period,
                DeliverableCode = actual.DeliverableCode,
                ActualVolume = actual.ActualVolume,
                ActualValue = actual.ActualValue,
                PeriodTypeCode = actual.PeriodTypeCode,
                ContractAllocationNumber = actual.ContractAllocationNumber
            });
        }
    }
}
