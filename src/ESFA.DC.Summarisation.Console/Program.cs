using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESF.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.ESF.Database.EF;
using ESFA.DC.ESF.Database.EF.Interfaces;
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
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.ESF.Data.Providers;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Main1819.Service.Tests.Stubs;
using ESFA.DC.Summarisation.Model;
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
            string esfConnectionString = @"Server=(local);Database=ESF;Trusted_Connection=True;";

            DbContextOptions<FcsContext> fcsdbContextOptions = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsConnectionString).Options;
            DbContextOptions<ILR1819_DataStoreEntities> ilrdbContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(ilrConnectionString).Options;
            DbContextOptions<EasContext> easdbContextOptions = new DbContextOptionsBuilder<EasContext>().UseSqlServer(easConnectionString).Options;
            DbContextOptions<SummarisationContext> sadbContextOptions = new DbContextOptionsBuilder<SummarisationContext>().UseSqlServer(summarisedActualsConnectionString).Options;

            DbContextOptions<ESF_DataStoreEntities> esfdbContextOptions = new DbContextOptionsBuilder<ESF_DataStoreEntities>().UseSqlServer(esfConnectionString).Options;

            IFcsContext fcsContext = new FcsContext(fcsdbContextOptions);
            IIlr1819RulebaseContext ilrContext = new ILR1819_DataStoreEntities(ilrdbContextOptions);
            IEasdbContext easContext = new EasContext(easdbContextOptions);
            SummarisationContext saContext = new SummarisationContext(sadbContextOptions);
            IESF_DataStoreEntities esfContext = new ESF_DataStoreEntities(esfdbContextOptions);

            IFcsRepository fcsRepository = new FcsRepository(fcsContext);

            ISummarisedActualsProcessRepository saRepository = new SummarisedActualsProcessRepository(saContext);

            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>()
            {
                new FundingTypesProvider(jsonSerializationService),
                new ESFA.DC.Summarisation.ESF.Service.FundingTypesProvider(jsonSerializationService)
            };

            List<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders = new List<ISummarisationConfigProvider<CollectionPeriod>>()
            {
                new CollectionPeriodsProvider(jsonSerializationService),
                new ESFA.DC.Summarisation.ESF.Service.CollectionPeriodsProvider(jsonSerializationService)
            };

            IProviderRepository repository = new ProviderRepository(new List<ILearningDeliveryProvider>
            {
                new AlbProvider(ilrContext),
                new EasProvider(easContext),
                new Fm25Provider(ilrContext),
                new Fm35Provider(ilrContext),
                new TblProvider(ilrContext),

                new ESFProvider_R1(esfContext),
                new ESFILRProvider(ilrContext)
            });

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess(),
                new SummarisationDeliverableProcess()
            };

            IBulkInsert bulkInsert = new BulkInsert();

            ISummarisedActualsPersist summarisedActualsPersist = new SummarisedActualsPersist(bulkInsert);

            ICollectionReturnMapper collectionReturnMapper = new CollectionReturnMapper();

            IDataStorePersistenceService dataStorePersistenceService = new DataStorePersistenceService(summarisedActualsPersist, collectionReturnMapper, () => new SqlConnection(summarisedActualsConnectionString));

            ILogger logger = new LoggerStub();

            ISummarisationContext summarisationMessage;

            SummarisationWrapper wrapper;

            summarisationMessage = new SummarisationContextStub();

            wrapper = new SummarisationWrapper(
                fcsRepository,
                saRepository,
                fundingTypesProviders,
                collectionPeriodsProviders,
                summarisationServices,
                dataStorePersistenceService,
                () => repository,
                new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                logger);

            await wrapper.Summarise(summarisationMessage, CancellationToken.None);

            summarisationMessage = new ESFSummarisationContextStub();

            wrapper = new SummarisationWrapper(
                fcsRepository,
                saRepository,
                fundingTypesProviders,
                collectionPeriodsProviders,
                summarisationServices,
                dataStorePersistenceService,
                () => repository,
                new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                logger);

            await wrapper.Summarise(summarisationMessage, CancellationToken.None);
        }
    }
}
