using Autofac;
using ESFA.DC.Summarisation.Apps.Apps2021.Service;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Apps2021.Modules
{
    public class Apps2021Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public Apps2021Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
        }
    }
}
