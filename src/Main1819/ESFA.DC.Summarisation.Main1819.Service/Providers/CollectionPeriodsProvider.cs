using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Service.Providers;

namespace ESFA.DC.Summarisation.Main1819.Service.Providers
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Main1819.Service.JsonFiles.CollectionPeriods.json";
    }
}
