using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Population.Service
{
    public class FcsRepository : IFcsRepository
    {
        private readonly IFcsContext _fcs;

        public FcsRepository(IFcsContext fcs)
        {
            _fcs = fcs;
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var contractAllocation = await _fcs.ContractAllocations
                .Select(ca => new FcsContractAllocation
                {
                    ContractAllocationNumber = ca.ContractAllocationNumber,
                    FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                    UoPcode = ca.UoPcode,
                    DeliveryUkprn = ca.DeliveryUkprn,
                    DeliveryOrganisation = ca.DeliveryOrganisation
                })
                .GroupBy(ca => ca.FundingStreamPeriodCode)
                .ToDictionaryAsync(gca => gca.Key, gca => gca.ToList() as IReadOnlyCollection<IFcsContractAllocation>, cancellationToken);

            return contractAllocation;
        }
    }
}
