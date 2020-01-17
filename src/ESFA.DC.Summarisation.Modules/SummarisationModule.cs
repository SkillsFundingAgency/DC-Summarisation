using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Autofac;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.ESF.Database.EF;
using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.ESF.R2.Database.EF;
using ESFA.DC.ESF.R2.Database.EF.Interfaces;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Configuration.Interface;
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
using ESFCollectionPeriodsProvider = ESFA.DC.Summarisation.ESF.ESF.Service.CollectionPeriodsProvider;
using ESFFundingTypesProvider = ESFA.DC.Summarisation.ESF.ESF.Service.FundingTypesProvider;
using ISummarisationContext = ESFA.DC.Summarisation.Model.Interface.ISummarisationContext;
using Main1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1920.Service.Providers.CollectionPeriodsProvider;
using Main1920FundingTypesProvider = ESFA.DC.Summarisation.Main1920.Service.Providers.FundingTypesProvider;
using Main1920Providers = ESFA.DC.Summarisation.Main1920.Service.Providers;
using Apps1920CollectionPeriodsProvider = ESFA.DC.Summarisation.Apps.Apps1920.Service.CollectionPeriodsProvider;
using Apps1920FundingTypesProvider = ESFA.DC.Summarisation.Apps.Apps1920.Service.FundingTypesProvider;
using ESFA.DC.ESF.FundingData.Database.EF.Interfaces;
using ESFA.DC.ESF.FundingData.Database.EF;
using System.Linq;
using ESFA.DC.Summarisation.Apps.Apps1920.Service.Providers;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert.Interface;
using ESFA.DC.Summarisation.ESF.ESF.Service.Providers;
using ESFA.DC.Summarisation.Main.Service;
using ESFA.DC.Summarisation.ESF.Service;
using ESFA.DC.Summarisation.Apps.Service;

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

            LoadSummarisationProcessModules(containerBuilder);

            LoadNewModules(containerBuilder);

            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<ILearningProvider>>();

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();

            LoadFCSModule(containerBuilder);

            LoadSummarisedActualsModules(containerBuilder);

            LoadILR1920Modules(containerBuilder);

            LoadESFModules(containerBuilder);

            LoadApps1920Modules(containerBuilder);
        }

        private void LoadSummarisationProcessModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationFundlineProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<SummarisationDeliverableProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<SummarisationPaymentsProcess>().As<ISummarisationService>();

            //containerBuilder.RegisterType<SummarisationFundlineProcess>().Keyed<ISummarisationService>(ProcessTypeConstants.Fundline);
            //containerBuilder.RegisterType<SummarisationDeliverableProcess>().Keyed<ISummarisationService>(ProcessTypeConstants.Deliverable);
            //containerBuilder.RegisterType<SummarisationPaymentsProcess>().Keyed<ISummarisationService>(ProcessTypeConstants.Payments);
        }

        private void LoadNewModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ProviderContractsService>().As<IProviderContractsService>();
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>();
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<ILearningProvider>>();
            containerBuilder.RegisterType<ProviderFundingDataRemovedService>().As<IProviderFundingDataRemovedService>();
        }

        private void LoadESFModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ESFFundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<ESFCollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<ESFProvider_R1>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<ESFProvider_R2>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<ESFILRProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();

            containerBuilder.RegisterType<ESF_DataStoreEntities>().As<IESF_DataStoreEntities>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ESF_DataStoreEntities>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ESFNonEFConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

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
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            }).As<DbContextOptions<ESFR2Context>>()
            .SingleInstance();

            containerBuilder.RegisterType<ESFFundingDataContext>().As<IESFFundingDataContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ESFFundingDataContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ESFFundingDataConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            }).As<DbContextOptions<ESFFundingDataContext>>()
            .SingleInstance();
        }

        private void LoadILR1920Modules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Main1920FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<Main1920CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<Main1920Providers.Fm35Provider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<Main1920Providers.EasProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<Main1920Providers.Fm25Provider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<Main1920Providers.AlbProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<Main1920Providers.TblProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();

            containerBuilder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.ILR1920ConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            }).As<DbContextOptions<ILR1920_DataStoreEntities>>()
            .SingleInstance();

            containerBuilder.RegisterType<EAS1920.EF.EasContext>().As<EAS1920.EF.Interface.IEasdbContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<EAS1920.EF.EasContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.EAS1920ConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            }).As<DbContextOptions<EAS1920.EF.EasContext>>()
            .SingleInstance();
        }

        private void LoadApps1920Modules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Apps1920FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<Apps1920CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<LevyProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.RegisterType<NonLevyProvider>().As<ISummarisationInputDataProvider<ILearningProvider>>();
            containerBuilder.Register(c =>
            {
                var factory = c.Resolve<Func<IDASPaymentsContext>>();

                var provider = c.Resolve<IEnumerable<ISummarisationConfigProvider<CollectionPeriod>>>().FirstOrDefault(p => p.CollectionType == "APPS");

                return new EasProvider(factory, provider);
            }).As<ISummarisationInputDataProvider<ILearningProvider>>();

            containerBuilder.RegisterType<DASPaymentsContext>().As<IDASPaymentsContext>().ExternallyOwned();
            containerBuilder.Register(c =>
            {
                var summarisationSettings = c.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<DASPaymentsContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.DASPaymentsConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;

            }).As<DbContextOptions<DASPaymentsContext>>()
            .SingleInstance();
        }

        private void LoadFCSModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FcsRepository>().As<IFcsRepository>();

            containerBuilder.RegisterType<FcsContext>().As<IFcsContext>().ExternallyOwned();
            containerBuilder.Register(context =>
            {
                var summarisationSettings = context.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<FcsContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.FCSConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            })
            .As<DbContextOptions<FcsContext>>()
            .SingleInstance();
        }

        private void LoadSummarisedActualsModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<BulkInsert>().As<IBulkInsert>();
            containerBuilder.RegisterType<SummarisedActualsPersist>().As<ISummarisedActualsPersist>();

            containerBuilder.RegisterType<SummarisedActualsProcessRepository>().As<IExistingSummarisedActualsRepository>();

            containerBuilder.RegisterType<DataStorePersistenceService>().As<IDataStorePersistenceService>();

            containerBuilder.Register(c => new SqlConnection(c.Resolve<ISummarisationDataOptions>().SummarisedActualsConnectionString)).As<SqlConnection>();

            containerBuilder.RegisterType<SummarisationContext>().As<ISummarisationContext>().ExternallyOwned();
            containerBuilder.Register(context =>
            {
                var summarisationSettings = context.Resolve<ISummarisationDataOptions>();
                var optionsBuilder = new DbContextOptionsBuilder<SummarisationContext>();
                optionsBuilder.UseSqlServer(
                    summarisationSettings.SummarisedActualsConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                        options.CommandTimeout(int.Parse(summarisationSettings.SqlCommandTimeoutSeconds));
                    });

                return optionsBuilder.Options;
            })
            .As<DbContextOptions<SummarisationContext>>()
            .SingleInstance();
        }
    }
}
