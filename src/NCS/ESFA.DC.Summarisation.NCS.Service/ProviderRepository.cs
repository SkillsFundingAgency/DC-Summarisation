using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.NCS.Service
{
    public class ProviderRepository : IInputDataRepository<TouchpointProviderFundingData>
    {
        private readonly IList<ISummarisationInputDataProvider> _providers;

        public ProviderRepository(IList<ISummarisationInputDataProvider> providers)
        {
            _providers = providers;
        }

        public async Task<TouchpointProviderFundingData> ProvideAsync(TouchpointProvider touchpointProvider, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providers = _providers;

            var taskResults = await Task.WhenAll(providers.Select(p => p.ProvideAsync(touchpointProvider, summarisationMessage, cancellationToken)));

            return new TouchpointProviderFundingData
            {
                Provider = touchpointProvider,
                FundingValues = taskResults.SelectMany(x => x).ToList()
            };
        }

        public async Task<ICollection<TouchpointProvider>> GetAllIdentifiersAsync(CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Select(p => p.ProvideUkprnsAsync(cancellationToken)));

            return taskResults.SelectMany(p => p).Distinct().ToList();
        }
    }
}