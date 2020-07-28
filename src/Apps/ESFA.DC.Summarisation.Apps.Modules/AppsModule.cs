using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.Summarisation.Apps.Apps1920.Modules;
using ESFA.DC.Summarisation.Apps.Apps2021.Modules;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Apps.Providers;
using ESFA.DC.Summarisation.Apps.Service;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Apps.Modules
{
    public class AppsModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public AppsModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<LearningProvider>>();
            containerBuilder.RegisterType<SummarisationPaymentsProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<LearningProvider>>();
            containerBuilder.RegisterType<ProviderContractsService>().As<IProviderContractsService>();

            containerBuilder.RegisterType<LevyProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<NonLevyProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<EasProvider>().As<ISummarisationInputDataProvider>();

            var AppsDataStoreEntitiesConfig = BuildDbContextOptions<DASPaymentsContext>(_summarisationDataOptions.DASPaymentsConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);
            containerBuilder.Register(c => new DASPaymentsContext(AppsDataStoreEntitiesConfig)).As<IDASPaymentsContext>().ExternallyOwned();

            containerBuilder.RegisterModule(new Apps1920Module(_summarisationDataOptions));
            containerBuilder.RegisterModule(new Apps2021Module(_summarisationDataOptions));
        }

        private DbContextOptions<T> BuildDbContextOptions<T>(string connectionString, string sqlCommandTimeOut) where T : DbContext
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