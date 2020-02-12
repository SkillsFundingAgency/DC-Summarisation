using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderFundingDataRemovedService
    {
        Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(string organisationId, ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(string organisationId, string uopCode, ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
