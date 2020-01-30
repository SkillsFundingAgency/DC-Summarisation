using Autofac;
using ESFA.DC.GenericCollection.EF;
using ESFA.DC.GenericCollection.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Generic.Service;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Generic.Modules
{
    public class GenericCollectionModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public GenericCollectionModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Generic);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<IEnumerable<Summarisation.Service.Model.SummarisedActual>>>();

            var dataStoreEntitiesConfig = BuildDbContextOptions<GenericCollectionContext>(_summarisationDataOptions.GenericCollectionsConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new GenericCollectionContext(dataStoreEntitiesConfig)).As<IGenericCollectionContext>().ExternallyOwned();

            containerBuilder.RegisterType<GenericCollectionRepository>().As<IGenericCollectionRepository>();
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