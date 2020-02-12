using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using ESFA.DC.Summarisation.NCS.NCS1920.Modules;
using ESFA.DC.Summarisation.NCS.Service;

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

            containerBuilder.RegisterModule(new NCS1920Module(_summarisationDataOptions));
        }
    }
}