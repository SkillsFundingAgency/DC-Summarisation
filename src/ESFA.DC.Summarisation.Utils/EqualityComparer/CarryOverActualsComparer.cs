using System;
using System.Collections.Generic;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Utils.EqualityComparer
{
    public class CarryOverActualsComparer : IEqualityComparer<SummarisedActual>
    {
        public bool Equals(SummarisedActual x, SummarisedActual y)
        {
            return x.DeliverableCode == y.DeliverableCode
                   && x.Period == y.Period
                   && x.FundingStreamPeriodCode.Equals(y.FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)
                   && x.ContractAllocationNumber.Equals(y.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase)
                   && x.OrganisationId.Equals(y.OrganisationId, StringComparison.OrdinalIgnoreCase);

        }

        public int GetHashCode(SummarisedActual obj)
        {   // Check whether the object is null. 
            if (obj == null)
            {
                return 0;
            }

            // Not hashing OrganisationId as usage of this is pre-filtered to be with same UKPRN, fields included are the key for Carry Over Actuals

            return (obj.OrganisationId,
                obj.DeliverableCode,
                obj.Period,
                obj.FundingStreamPeriodCode,
                obj.ContractAllocationNumber).GetHashCode();
        }
    }
}
