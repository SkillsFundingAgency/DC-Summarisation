using Autofac;
using ESFA.DC.ESF.Database.EF;
using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.ESF.FundingData.Database.EF;
using ESFA.DC.ESF.FundingData.Database.EF.Interfaces;
using ESFA.DC.ESF.R2.Database.EF;
using ESFA.DC.ESF.R2.Database.EF.Interfaces;
using ESFA.DC.Summarisation.ESF.ESF.Service;
using ESFA.DC.Summarisation.ESF.ESF.Service.Providers;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.ESF.Modules
{
    public class ESFESFModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;
        
        public ESFESFModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<ESFProvider_R1>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<ESFProvider_R2>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<ESFILRProvider>().As<ISummarisationInputDataProvider<LearningProvider>>();
            
            var esfDataStoreEntitiesConfig = BuildDbContextOptions<ESF_DataStoreEntities>(_summarisationDataOptions.ESFNonEFConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new ESF_DataStoreEntities(esfDataStoreEntitiesConfig)).As<IESF_DataStoreEntities>().ExternallyOwned();

            var esfR2DataStoreEntitiesConfig = BuildDbContextOptions<ESFR2Context>(_summarisationDataOptions.ESFR2ConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new ESFR2Context(esfR2DataStoreEntitiesConfig)).As<IESFR2Context>().ExternallyOwned();

            var esfFundingDataStoreEntitiesConfig = BuildDbContextOptions<ESFFundingDataContext>(_summarisationDataOptions.ESFFundingDataConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new ESFFundingDataContext(esfFundingDataStoreEntitiesConfig)).As<IESFFundingDataContext>().ExternallyOwned();

        }

        private DbContextOptions<T> BuildDbContextOptions<T>(string connectionString, string sqlCommandTimeOut)
            where T : DbContext
        {            
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            optionsBuilder.UseSqlServer(
                connectionString,
                options =>
                {
                    options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                    options.CommandTimeout(int.Parse(sqlCommandTimeOut));
                });

            return optionsBuilder.Options;
        }
    }
}
