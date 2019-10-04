using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESF.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.ESF.Database.EF;
using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.ESF.R2.Database.EF;
using ESFA.DC.ESF.R2.Database.EF.Interfaces;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Apps1819.Data;
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
using Main1920Providers = ESFA.DC.Summarisation.Main1920.Data.Providers;
using Main1920FundingTypesProvider = ESFA.DC.Summarisation.Main1920.Service.Providers;
using Main1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1920.Service.Providers;
using Apps1920Providers = ESFA.DC.Summarisation.Apps1920.Data;
using Apps1920FundingTypesProvider = ESFA.DC.Summarisation.Apps1920.Service;
using Apps1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Apps1920.Service;
using ESFA.DC.ESF.FundingData.Database.EF;

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
            string ilr1819ConnectionString = @"Server=(local);Database=ILR1819DataStore;Trusted_Connection=True;";
            string ilr1920ConnectionString = @"Server=(local);Database=ILR1920DataStore;Trusted_Connection=True;";

            string summarisedActualsConnectionString = @"Server=(local);Database=SummarisedActuals;Trusted_Connection=True;";
            string eas1819ConnectionString = @"Server=(local);Database=EAS1819;Trusted_Connection=True;";
            string eas1920ConnectionString = @"Server=(local);Database=EAS1920;Trusted_Connection=True;";

            string esfConnectionString = @"Server=(local);Database=ESF;Trusted_Connection=True;";
            string esfR2ConnectionString = @"Server=(local);Database=ESF-R2;Trusted_Connection=True;";

            string dasConnectionString = @"Server=(local);Database=DASPayments;Trusted_Connection=True;";

            string esfFundingDataConnectionString = @"Server=(local);Database=ESFFundingData;Trusted_Connection=True;";

            DbContextOptions<FcsContext> fcsdbContextOptions = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsConnectionString).Options;
            DbContextOptions<ILR1819_DataStoreEntities> ilr1819dbContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(ilr1819ConnectionString).Options;
            DbContextOptions<ILR1920_DataStoreEntities> ilr1920dbContextOptions = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>().UseSqlServer(ilr1920ConnectionString).Options;

            DbContextOptions<EAS1819.EF.EasContext> eas1819dbContextOptions = new DbContextOptionsBuilder<EAS1819.EF.EasContext>().UseSqlServer(eas1819ConnectionString).Options;
            DbContextOptions<EAS1920.EF.EasContext> eas1920dbContextOptions = new DbContextOptionsBuilder<EAS1920.EF.EasContext>().UseSqlServer(eas1920ConnectionString).Options;

            DbContextOptions<SummarisationContext> sadbContextOptions = new DbContextOptionsBuilder<SummarisationContext>().UseSqlServer(summarisedActualsConnectionString).Options;
            DbContextOptions<ESF_DataStoreEntities> esfdbContextOptions = new DbContextOptionsBuilder<ESF_DataStoreEntities>().UseSqlServer(esfConnectionString).Options;
            DbContextOptions<ESFR2Context> esfR2dbContextOptions = new DbContextOptionsBuilder<ESFR2Context>().UseSqlServer(esfR2ConnectionString).Options;
            DbContextOptions<ESFFundingDataContext> esfFddbContextOptions = new DbContextOptionsBuilder<ESFFundingDataContext>().UseSqlServer(esfFundingDataConnectionString).Options;

            DbContextOptions<DASPaymentsContext> dasdbContextOptions = new DbContextOptionsBuilder<DASPaymentsContext>().UseSqlServer(dasConnectionString).Options;

            IFcsRepository fcsRepository = new FcsRepository(() => new FcsContext(fcsdbContextOptions));
            ISummarisedActualsProcessRepository saRepository
                = new SummarisedActualsProcessRepository(() => new SummarisationContext(sadbContextOptions));

            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>()
            {
                new FundingTypesProvider(jsonSerializationService),
                new ESF.Service.FundingTypesProvider(jsonSerializationService),
                new Apps1819.Service.FundingTypesProvider(jsonSerializationService),
                new Main1920FundingTypesProvider.FundingTypesProvider(jsonSerializationService),
                new Apps1920FundingTypesProvider.FundingTypesProvider(jsonSerializationService),
            };

            List<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders
                = new List<ISummarisationConfigProvider<CollectionPeriod>>()
            {
                new CollectionPeriodsProvider(jsonSerializationService),
                new ESF.Service.CollectionPeriodsProvider(jsonSerializationService),
                new Apps1819.Service.CollectionPeriodsProvider(jsonSerializationService),
                new Main1920CollectionPeriodsProvider.CollectionPeriodsProvider(jsonSerializationService),
                new Apps1920CollectionPeriodsProvider.CollectionPeriodsProvider(jsonSerializationService),
            };

            IProviderRepository repository = new ProviderRepository(new List<ILearningDeliveryProvider>
            {
                new AlbProvider(() => new ILR1819_DataStoreEntities(ilr1819dbContextOptions)),
                new EasProvider(() => new EAS1819.EF.EasContext(eas1819dbContextOptions)),
                new Fm25Provider(() => new ILR1819_DataStoreEntities(ilr1819dbContextOptions)),
                new Fm35Provider(() => new ILR1819_DataStoreEntities(ilr1819dbContextOptions)),
                new TblProvider(() => new ILR1819_DataStoreEntities(ilr1819dbContextOptions)),

                new ESFProvider_R1(() => new ESF_DataStoreEntities(esfdbContextOptions)),
                new ESFProvider_R2(() => new ESFR2Context(esfR2dbContextOptions)),
                new ESFILRProvider(() => new ESFFundingDataContext(esfFddbContextOptions)),

                new LevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),
                new NonLevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),

                new Main1920Providers.Fm35Provider(() => new ILR1920_DataStoreEntities(ilr1920dbContextOptions)),
                new Main1920Providers.EasProvider(() => new EAS1920.EF.EasContext(eas1920dbContextOptions)),

                new Apps1920Providers.LevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),
                new Apps1920Providers.NonLevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),
                new Apps1920Providers.EasProvider(() => new DASPaymentsContext(dasdbContextOptions), new Apps1920CollectionPeriodsProvider.CollectionPeriodsProvider(jsonSerializationService)),
            });

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess(),
                new SummarisationDeliverableProcess(),
                new SummarisationPaymentsProcess(),
            };

            IBulkInsert bulkInsert = new BulkInsert();

            ISummarisedActualsPersist summarisedActualsPersist = new SummarisedActualsPersist(bulkInsert);

            ICollectionReturnMapper collectionReturnMapper = new CollectionReturnMapper();

            IDataStorePersistenceService dataStorePersistenceService = new DataStorePersistenceService(summarisedActualsPersist, collectionReturnMapper, () => new SqlConnection(summarisedActualsConnectionString));

            ILogger logger = new LoggerStub();

            ISummarisationMessage summarisationMessage;

            SummarisationWrapper wrapper;

            /* summarisationMessage = new SummarisationContextStub();

             wrapper = new SummarisationWrapper(
                 fcsRepository,
                 saRepository,
                 fundingTypesProviders,
                 collectionPeriodsProviders,
                 summarisationServices,
                 dataStorePersistenceService,
                 () => repository,
                 new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                 logger,
                 summarisationMessage);

             await wrapper.Summarise(CancellationToken.None);

             summarisationMessage = new ILR1920SummarisationContextStub();

             wrapper = new SummarisationWrapper(
                 fcsRepository,
                 saRepository,
                 fundingTypesProviders,
                 collectionPeriodsProviders,
                 summarisationServices,
                 dataStorePersistenceService,
                 () => repository,
                 new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                 logger,
                 summarisationMessage);

             await wrapper.Summarise(CancellationToken.None);

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
                logger,
                summarisationMessage);

            await wrapper.Summarise(CancellationToken.None);*/

            summarisationMessage = new Apps1920SummarisationContextStub();

            wrapper = new SummarisationWrapper(
                fcsRepository,
                saRepository,
                fundingTypesProviders,
                collectionPeriodsProviders,
                summarisationServices,
                dataStorePersistenceService,
                () => repository,
                new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
                logger,
                summarisationMessage);

            await wrapper.Summarise(CancellationToken.None);
        }
    }
}
