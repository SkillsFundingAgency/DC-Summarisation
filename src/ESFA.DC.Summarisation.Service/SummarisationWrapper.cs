using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SummarisedActual = ESFA.DC.Summarisation.Data.output.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly ILogger _logger;
        private readonly ISummarisationProcess _summarisationProcess;

        public SummarisationWrapper(
            IDataStorePersistenceService dataStorePersistenceService,
            ILogger logger,
            ISummarisationProcess summarisationProcess)
        {
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _summarisationProcess = summarisationProcess;

        }

        public async Task<ICollection<SummarisedActual>> Summarise(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var summarisedActuals = await _summarisationProcess.CollateAndSummariseAsync(summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");

            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(
                summarisedActuals.ToList(),
                summarisationMessage,
                cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }
    }
}
