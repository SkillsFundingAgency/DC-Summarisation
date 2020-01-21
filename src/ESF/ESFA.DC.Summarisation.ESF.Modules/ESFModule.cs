using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Service;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Modules
{
    public class ESFModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<ILearningProvider>>();
            containerBuilder.RegisterType<SummarisationDeliverableProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<ILearningProvider>>();

        }
    }
}