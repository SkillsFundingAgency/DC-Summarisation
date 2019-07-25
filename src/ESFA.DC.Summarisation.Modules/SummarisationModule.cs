using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Autofac;
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
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.Summarisation.Apps1819.Data;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Configuration.Interface;
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
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;
using ESFCollectionPeriodsProvider = ESFA.DC.Summarisation.ESF.Service.CollectionPeriodsProvider;
using ESFFundingTypesProvider = ESFA.DC.Summarisation.ESF.Service.FundingTypesProvider;
using ISummarisationContext = ESFA.DC.Summarisation.Model.Interface.ISummarisationContext;
using Main1819CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1819.Service.Providers.CollectionPeriodsProvider;
using Main1819FundingTypesProvider = ESFA.DC.Summarisation.Main1819.Service.Providers.FundingTypesProvider;
using Main1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1920.Service.Providers.CollectionPeriodsProvider;
using Main1920FundingTypesProvider = ESFA.DC.Summarisation.Main1920.Service.Providers.FundingTypesProvider;
using Main1920Providers = ESFA.DC.Summarisation.Main1920.Data.Providers;

namespace ESFA.DC.Summarisation.Modules
{
    public class SummarisationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            var configHelper = new ConfigurationHelper();

            var referenceDataOptions = configHelper.GetSectionValues<SummarisationDataOptions>("ReferenceDataSection");
            containerBuilder.RegisterInstance(referenceDataOptions).As<ISummarisationDataOptions>().SingleInstance();

            containerBuilder.RegisterType<SummarisationWrapper>().As<ISummarisationWrapper>();
            containerBuilder.RegisterType<SummarisationFundlineProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<SummarisationDeliverableProcess>().As<ISummarisationService>();

            containerBuilder.RegisterType<Main1819FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<ESFFundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<Apps1819.Service.FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();

            containerBuilder.RegisterType<Main1819CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
            containerBuilder.RegisterType<ESFCollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
            containerBuilder.RegisterType<Apps1819.Service.CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<AlbProvider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<TblProvider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<Fm25Provider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<Fm35Provider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<EasProvider>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<ESFProvider_R1>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<ESFProvider_R2>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<ESFILRProvider>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<LevyProvider>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<ProviderRepository>().As<IProviderRepository>();

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<FcsRepository>().As<IFcsRepository>();
            containerBuilder.RegisterType<SummarisedActualsProcessRepository>().As<ISummarisedActualsProcessRepository>();

            containerBuilder.RegisterType<BulkInsert>().As<IBulkInsert>();
            containerBuilder.RegisterType<SummarisedActualsPersist>().As<ISummarisedActualsPersist>();

            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();

            containerBuilder.RegisterType<DataStorePersistenceService>().As<IDataStorePersistenceService>();

            containerBuilder.Register(c => new SqlConnection(c.Resolve<ISummarisationDataOptions>().SummarisedActualsConnectionString)).As<SqlConnection>();

            containerBuilder.RegisterType<FcsContext>().As<IFcsContext>().ExternallyOwned();
            containerBuilder.Register(context =>
            {
                var summarisationSettings = context.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<FcsContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.FCSConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            })
            .As<DbContextOptions<FcsContext>>()
            .SingleInstance();

            containerBuilder.RegisterType<ILR1819_DataStoreEntities>().As<IIlr1819RulebaseContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ILR1819ConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            }).As<DbContextOptions<ILR1819_DataStoreEntities>>()
            .SingleInstance();

            containerBuilder.RegisterType<EasContext>().As<IEasdbContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<EasContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.EAS1819ConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            }).As<DbContextOptions<EasContext>>()
            .SingleInstance();

            containerBuilder.RegisterType<ESF_DataStoreEntities>().As<IESF_DataStoreEntities>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ESF_DataStoreEntities>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ESFNonEFConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            }).As<DbContextOptions<ESF_DataStoreEntities>>()
            .SingleInstance();

            containerBuilder.RegisterType<ESFR2Context>().As<IESFR2Context>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ESFR2Context>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ESFR2ConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            }).As<DbContextOptions<ESFR2Context>>()
            .SingleInstance();

            containerBuilder.RegisterType<DASPaymentsContext>().As<IDASPaymentsContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<DASPaymentsContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.DASPaymentsConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;

            }).As<DbContextOptions<DASPaymentsContext>>()
            .SingleInstance();

            containerBuilder.RegisterType<SummarisationContext>().As<ISummarisationContext>().ExternallyOwned();
            containerBuilder.Register(context =>
            {
                var summarisationSettings = context.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<SummarisationContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.SummarisedActualsConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            })
            .As<DbContextOptions<SummarisationContext>>()
            .SingleInstance();

            LoadILR1920Modules(containerBuilder);
        }

        private void LoadILR1920Modules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Main1920FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<Main1920CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<Main1920Providers.Fm35Provider>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ILR1920ConnectionString,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            }).As<DbContextOptions<ILR1920_DataStoreEntities>>()
            .SingleInstance();

        }
    }
}
