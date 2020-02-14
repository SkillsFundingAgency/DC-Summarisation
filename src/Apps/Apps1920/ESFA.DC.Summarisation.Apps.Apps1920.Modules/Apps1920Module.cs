using Autofac;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.Summarisation.Apps.Apps1920.Service;
using ESFA.DC.Summarisation.Apps.Apps1920.Service.Providers;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Apps.Apps1920.Modules
{
    public class Apps1920Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public Apps1920Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<LevyProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<NonLevyProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<EasProvider>().As<ISummarisationInputDataProvider>();

            var AppsDataStoreEntitiesConfig = BuildDbContextOptions<DASPaymentsContext>(_summarisationDataOptions.DASPaymentsConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new DASPaymentsContext(AppsDataStoreEntitiesConfig)).As<IDASPaymentsContext>().ExternallyOwned();
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
