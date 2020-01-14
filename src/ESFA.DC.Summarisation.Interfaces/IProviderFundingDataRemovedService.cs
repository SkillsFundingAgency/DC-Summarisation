using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderFundingDataRemovedService
    {
        Task<IEnumerable<SummarisedActual>> FundingDataRemovedAsync(List<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
