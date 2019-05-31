using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Service.Providers;
using System.Reflection;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => nameof(Configuration.Enum.CollectionType.ESF);

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.ESF.Service.JsonFiles.CollectionPeriods.json";

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    }
}