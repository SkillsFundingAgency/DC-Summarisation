using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Population.Service
{
    public class FCSDataRetrievalService
    {
        /// <summary>
        /// The FCS (DB Context)
        /// </summary>
        private readonly IFcsContext _fcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataRetrievalService"/> class
        /// </summary>
        /// <param name="fcs">The FCS.</param>
        public FCSDataRetrievalService(IFcsContext fcs)
        {
            _fcs = fcs;
        }

        /// <summary>
        /// Retrieves the (FCS Contracts) asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder.
        /// </returns>
        /// public async Task<IReadOnlyCollection<IFcsContractAllocation>> RetrieveAsync(CancellationToken cancellationToken)
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
