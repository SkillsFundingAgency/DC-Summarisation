using Autofac;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using ESFA.DC.Summarisation.NCS.NCS1920.Modules;
using ESFA.DC.Summarisation.NCS.NCS2021.Modules;
using ESFA.DC.Summarisation.NCS.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.NCS.Modules
{
    public class NCSModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public NCSModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Ncs);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<TouchpointProviderFundingData>>();
            containerBuilder.RegisterType<SummarisationNCSProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<TouchpointProviderFundingData>>();
            containerBuilder.RegisterType<ProviderContractsService>().As<IProviderContractsService>();
            containerBuilder.RegisterType<NCSProvider>().As<ISummarisationInputDataProvider>();

            var NCSDataStoreEntitiesConfig = BuildDbContextOptions<NcsContext>(_summarisationDataOptions.NcsDbConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new NcsContext(NCSDataStoreEntitiesConfig)).As<INcsContext>().ExternallyOwned();

            containerBuilder.RegisterModule(new NCS1920Module());

            containerBuilder.RegisterModule(new NCS2021Module());
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