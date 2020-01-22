using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.ESF.ESF.Modules;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.ESF.Service;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Modules
{
    public class ESFModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public ESFModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<LearningProvider>>();
            containerBuilder.RegisterType<SummarisationDeliverableProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<LearningProvider>>();

            containerBuilder.RegisterModule(new ESFESFModule(_summarisationDataOptions));
        }
    }
}