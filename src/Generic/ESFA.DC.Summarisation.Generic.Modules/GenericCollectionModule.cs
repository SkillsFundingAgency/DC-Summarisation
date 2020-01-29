using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Generic.Service;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Apps.Modules
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
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<IEnumerable<SummarisedActual>>>();
        }
    }
}