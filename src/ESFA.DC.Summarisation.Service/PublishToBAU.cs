using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Service
{
    public class PublishToBAU : IPublishToBAU
    {
        private readonly IDataStorePersistenceServiceBAU _dataStorePersistenceService;
        private readonly IExistingSummarisedActualsRepository _summarisedActualsRepository;
        private readonly ISummarisedActualBAUMapper _summarisedActualMapper;
        private readonly IEventLogMapper _eventLogMapper;
        private readonly ILogger _logger;

        public PublishToBAU(
            IDataStorePersistenceServiceBAU dataStorePersistenceService,
            IExistingSummarisedActualsRepository summarisedActualsRepository,
            ISummarisedActualBAUMapper summarisedActualMapper,
            IEventLogMapper eventLogMapper,
            ILogger logger)
        {
            _dataStorePersistenceService = dataStorePersistenceService;
            _summarisedActualsRepository = summarisedActualsRepository;
            _summarisedActualMapper = summarisedActualMapper;
            _eventLogMapper = eventLogMapper;
            _logger = logger;

        }

        public async Task Publish(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var collectionReturn = await _summarisedActualsRepository.GetLastCollectionReturnAsync(summarisationMessage.CollectionType, summarisationMessage.CollectionReturnCode, cancellationToken);

            var eventLog = _eventLogMapper.Map(collectionReturn);

            _logger.LogInfo($"Publish to BAU: Retrieving and storing data to BAU Start");

            var summarisedActuals = await _summarisedActualsRepository.GetSummarisedActualsAsync(collectionReturn.Id, cancellationToken);

            var summarisedActualsBAU = _summarisedActualMapper.Map(summarisedActuals, summarisationMessage.CollectionType, summarisationMessage.CollectionReturnCode);

            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(
                summarisedActualsBAU,
                eventLog,
                cancellationToken);

            _logger.LogInfo($"Publish to BAU: Retrieving and storing data to BAU End");
        }
    }
}
