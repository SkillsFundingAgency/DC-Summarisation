using Autofac;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.NCS.NCS2021.Service.Providers;

namespace ESFA.DC.Summarisation.NCS.NCS2021.Modules
{
    public class NCS2021Module : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
        }
       
    }
}
