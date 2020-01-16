using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Service.Providers;

namespace ESFA.DC.Summarisation.ESF.ESF.Service
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.ESF;

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.ESF.ESF.Service.JsonFiles.CollectionPeriods.json";

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    }
}