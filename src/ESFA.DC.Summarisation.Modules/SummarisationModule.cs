using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;
using ISummarisationContext = ESFA.DC.Summarisation.Model.Interface.ISummarisationContext;
using ESFA.DC.Summarisation.Main.Modules;
using ESFA.DC.Summarisation.ESF.Modules;
using ESFA.DC.Summarisation.Apps.Modules;

namespace ESFA.DC.Summarisation.Modules
{
    public class SummarisationModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public SummarisationModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            var configHelper = new ConfigurationHelper();

            containerBuilder.RegisterInstance(_summarisationDataOptions).As<ISummarisationDataOptions>().SingleInstance();

            containerBuilder.RegisterType<SummarisationWrapper>().As<ISummarisationWrapper>();

            LoadSummarisationProcessModules(containerBuilder);

            LoadNewModules(containerBuilder);

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();

            LoadFCSModule(containerBuilder);

            LoadSummarisedActualsModules(containerBuilder);

        }

        private void LoadSummarisationProcessModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new MainModule(_summarisationDataOptions));

            containerBuilder.RegisterModule(new ESFModule(_summarisationDataOptions));

            containerBuilder.RegisterModule(new AppsModule(_summarisationDataOptions));

            containerBuilder.RegisterModule<PersistenceModule>();
        }

        private void LoadNewModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ProviderContractsService>().As<IProviderContractsService>();
            containerBuilder.RegisterType<ProviderFundingDataRemovedService>().As<IProviderFundingDataRemovedService>();
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
            containerBuilder.RegisterType<SummarisedActualsProcessRepository>().As<IExistingSummarisedActualsRepository>();

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
