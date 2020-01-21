using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
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
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Console.Stubs;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;
using Main1920Providers = ESFA.DC.Summarisation.Main1920.Service.Providers;
using Main1920FundingTypesProvider = ESFA.DC.Summarisation.Main1920.Service.Providers;
using Main1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1920.Service.Providers;
using ESFA.DC.ESF.FundingData.Database.EF;
using ESFA.DC.Summarisation.Apps.Apps1920.Service;
using ESFA.DC.Summarisation.Apps.Apps1920.Service.Providers;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert.Interface;
using ESFA.DC.Summarisation.ESF.ESF.Service.Providers;
using ESFA.DC.Summarisation.Stateless.Config;

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

            var summarisationDataOptions = new SummarisationDataOptions() { SummarisedActualsConnectionString = summarisedActualsConnectionString };

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
            IExistingSummarisedActualsRepository saRepository
                = new SummarisedActualsProcessRepository(() => new SummarisationContext(sadbContextOptions));

            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>()
            {
                new ESF.ESF.Service.FundingTypesProvider(jsonSerializationService),
                new Main1920FundingTypesProvider.FundingTypesProvider(jsonSerializationService),
                new FundingTypesProvider(jsonSerializationService),
            };

            List<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders = new List<ISummarisationConfigProvider<CollectionPeriod>>()
            {
                new ESF.ESF.Service.CollectionPeriodsProvider(jsonSerializationService),
                new Main1920CollectionPeriodsProvider.CollectionPeriodsProvider(jsonSerializationService),
                new CollectionPeriodsProvider(jsonSerializationService),
            };

            var summarisationInputDataProviders = new List<ISummarisationInputDataProvider<ILearningProvider>>
            {
                new ESFProvider_R1(() => new ESF_DataStoreEntities(esfdbContextOptions)),
                new ESFProvider_R2(() => new ESFR2Context(esfR2dbContextOptions)),
                new ESFILRProvider(() => new ESFFundingDataContext(esfFddbContextOptions)),

                new Main1920Providers.Fm35Provider(() => new ILR1920_DataStoreEntities(ilr1920dbContextOptions)),
                new Main1920Providers.EasProvider(() => new EAS1920.EF.EasContext(eas1920dbContextOptions)),

                new LevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),
                new NonLevyProvider(() => new DASPaymentsContext(dasdbContextOptions)),
                new EasProvider(() => new DASPaymentsContext(dasdbContextOptions), new CollectionPeriodsProvider(jsonSerializationService)),
            };

            IInputDataRepository<ILearningProvider> repository = new ProviderRepository(summarisationInputDataProviders);

            IBulkInsert bulkInsert = new BulkInsert();

            ISummarisedActualsPersist summarisedActualsPersist = new SummarisedActualsPersist(bulkInsert);

            ICollectionReturnMapper collectionReturnMapper = new CollectionReturnMapper();

            IDataStorePersistenceService dataStorePersistenceService = new DataStorePersistenceService(summarisedActualsPersist, collectionReturnMapper, summarisationDataOptions);

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
                 logger);

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
                 logger);

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
                logger);

            await wrapper.Summarise(CancellationToken.None);*/

            summarisationMessage = new Apps1920SummarisationContextStub();

            //wrapper = new SummarisationWrapper(
            //    fcsRepository,
            //    saRepository,
            //    fundingTypesProviders,
            //    collectionPeriodsProviders,
            //    summarisationServices,
            //    dataStorePersistenceService,
            //    () => repository,
            //    new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" },
            //    logger
            //    );

            //await wrapper.Summarise(summarisationMessage, CancellationToken.None);
        }
    }
}
