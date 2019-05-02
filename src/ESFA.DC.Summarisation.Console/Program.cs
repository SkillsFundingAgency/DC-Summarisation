using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESF.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Console.Stubs;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Population.Service;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Main1819.Service.Tests.Stubs;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.WaitAll(RunSummarisation());
        }

        private static async Task RunSummarisation()
        {
            string fcsConnectionString = @"Server=(local);Database=FCS;Trusted_Connection=True;";
            string ilrConnectionString = @"Server=(local);Database=ILR1819DataStore;Trusted_Connection=True;";

            string summarisedActualsConnectionString = @"Server=(local);Database=SummarisedActuals;Trusted_Connection=True;";
            string easConnectionString = @"Server=(local);Database=EAS1819;Trusted_Connection=True;";

            DbContextOptions<FcsContext> fcsdbContextOptions = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsConnectionString).Options;
            DbContextOptions<ILR1819_DataStoreEntities> ilrdbContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(ilrConnectionString).Options;
            DbContextOptions<EasContext> easdbContextOptions = new DbContextOptionsBuilder<EasContext>().UseSqlServer(easConnectionString).Options;

            IFcsContext fcsContext = new FcsContext(fcsdbContextOptions);
            IIlr1819RulebaseContext ilrContext = new ILR1819_DataStoreEntities(ilrdbContextOptions);
            IEasdbContext easContext = new EasContext(easdbContextOptions);

            IFcsRepository fcsRepository = new FcsRepository(fcsContext);

            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            ISummarisationConfigProvider<FundingType> fundingTypesProvider = new FundingTypesProvider(jsonSerializationService);

            ISummarisationConfigProvider<CollectionPeriod> collectionPeriodsProvider = new CollectionPeriodsProvider(jsonSerializationService);

            IProviderRepository repository = new ProviderRepository(new List<ILearningDeliveryProvider>
            {
                new AlbProvider(ilrContext),
                new EasProvider(easContext),
                new Fm35Provider(ilrContext),
                new TblProvider(ilrContext)
            });

            ISummarisationService summarisationService = new SummarisationService();

            IBulkInsert bulkInsert = new BulkInsert();

            ISummarisedActualsPersist summarisedActualsPersist = new SummarisedActualsPersist(bulkInsert);

            ICollectionReturnMapper collectionReturnMapper = new CollectionReturnMapper();

            IDataStorePersistenceService dataStorePersistenceService = new DataStorePersistenceService(summarisedActualsPersist, collectionReturnMapper, () => new SqlConnection(summarisedActualsConnectionString));

            ILogger logger = new LoggerStub();

            var summarisationMessage = new SummarisationContextStub();

            SummarisationWrapper wrapper = new SummarisationWrapper(
                fcsRepository,
                fundingTypesProvider,
                collectionPeriodsProvider,
                summarisationService,
                dataStorePersistenceService,
                () => repository,
                new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                logger);

            await wrapper.Summarise(summarisationMessage, CancellationToken.None);

        }
    }
}
