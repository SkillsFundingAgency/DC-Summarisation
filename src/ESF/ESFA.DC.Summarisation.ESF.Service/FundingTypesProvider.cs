using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Service.Providers;
using System.Reflection;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.ESF.Service.JsonFiles.FundingTypes.json";

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public override string CollectionType => nameof(Configuration.Enum.CollectionType.ESF);
    }
}

