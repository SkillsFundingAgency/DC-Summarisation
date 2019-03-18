﻿using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Population.Service;
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Service;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Model;

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

            DbContextOptions<FcsContext> fcsdbContextOptions = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsConnectionString).Options;
            DbContextOptions<ILR1819_DataStoreEntities> ilrdbContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(ilrConnectionString).Options;

            DbContextOptions<SummarisationContext> summarisationDbContextOptions = new DbContextOptionsBuilder<SummarisationContext>().UseSqlServer(summarisedActualsConnectionString).Options;

            IFcsContext fcsContext = new FcsContext(fcsdbContextOptions);

            IFcsRepository fcsRepository = new FcsRepository(fcsContext);

            SummarisationContext summarisationContext = new SummarisationContext(summarisationDbContextOptions);

            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            IStaticDataProvider<FundingType> fundingTypesProvider = new FundingTypesProvider(jsonSerializationService);

            IStaticDataProvider<CollectionPeriod> collectionPeriodsProvider = new CollectionPeriodsProvider(jsonSerializationService);

            IIlr1819RulebaseContext ilr1819RulebaseContext = new ILR1819_DataStoreEntities(ilrdbContextOptions);

            ICollection<IProviderRepository> repositories = new List<IProviderRepository>() { new Fm35Repository(ilr1819RulebaseContext) };

            ISummarisationService summarisationService = new SummarisationService();

            IBulkInsert bulkInsert = new BulkInsert();
            ISummarisedActualsMapper summarisedActualsMapper = new SummarisedActualsMapper();
            ISummarisedActualsPersist summarisedActualsPersist = new SummarisedActualsPersist(bulkInsert, new SqlConnection(summarisedActualsConnectionString), summarisedActualsMapper);

            ICollectionReturnMapper collectionReturnMapper = new CollectionReturnMapper();
            ICollectionReturnPersist collectionReturnPersist = new CollectionReturnPersist(collectionReturnMapper, summarisationContext);

            IDataStorePersistenceService dataStorePersistenceService = new DataStorePersistenceService(summarisedActualsPersist, collectionReturnPersist);

            var summarisationMessage = new SummarisationMessage { CollectionType = "ILR1819", CollectionReturnCode = "R01" };
        
            SummarisationWrapper wrapper = new SummarisationWrapper(fcsRepository,
                fundingTypesProvider,
                collectionPeriodsProvider,
                repositories,
                summarisationService,
                dataStorePersistenceService,
                new SqlConnection(summarisedActualsConnectionString));

            List<string> fundModels = new List<string> { "FM35" };

            var result = await wrapper.Summarise(fundModels, summarisationMessage, CancellationToken.None, false);
        }
    }
}