using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.ESF.Service;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Modules
{
    public class ESFModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
        }
    }
}