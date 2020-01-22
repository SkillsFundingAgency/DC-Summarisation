using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Service.Model.Config;
using ESFA.DC.Summarisation.Service.Providers;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.ILR1920;

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Main.Main1920.Service.JsonFiles.FundingTypes.json";
    }
}
