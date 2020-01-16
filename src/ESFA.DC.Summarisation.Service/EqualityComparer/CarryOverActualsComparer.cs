using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Summarisation.Data.output.Model;

namespace ESFA.DC.Summarisation.Service.EqualityComparer
{
    public class CarryOverActualsComparer : IEqualityComparer<SummarisedActual>
    {
        public bool Equals(SummarisedActual x, SummarisedActual y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(SummarisedActual obj)
        {
            // Check whether the object is null. 
            if (obj == null)
            {
                return 0;
            }

            // Not hashing OrganisationId as usage of this is pre-filtered to be with same UKPRN, fields included are the key for Carry Over Actuals

            return (obj.DeliverableCode,
                obj.Period,
                obj.FundingStreamPeriodCode,
                obj.ContractAllocationNumber).GetHashCode();
        }
    }
}
