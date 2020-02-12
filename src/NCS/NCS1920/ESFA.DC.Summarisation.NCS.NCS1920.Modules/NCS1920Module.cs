using Autofac;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.NCS.NCS1920.Service;
using ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.NCS.NCS1920.Modules
{
    public class NCS1920Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public NCS1920Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
            containerBuilder.RegisterType<NCSProvider>().As<ISummarisationInputDataProvider>();

            var NCSDataStoreEntitiesConfig = BuildDbContextOptions<NcsContext>(_summarisationDataOptions.NcsDbConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new NcsContext(NCSDataStoreEntitiesConfig)).As<INcsContext>().ExternallyOwned();
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
