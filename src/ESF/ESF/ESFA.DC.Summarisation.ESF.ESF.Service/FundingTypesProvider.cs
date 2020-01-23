using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Service.Model.Config;

namespace ESFA.DC.Summarisation.ESF.ESF.Service
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.ESF.ESF.Service.JsonFiles.FundingTypes.json";

        protected internal override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public override string CollectionType => CollectionTypeConstants.ESF;
    }
}

