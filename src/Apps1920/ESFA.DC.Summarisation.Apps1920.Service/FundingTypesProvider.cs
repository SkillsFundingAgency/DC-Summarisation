﻿using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Service.Providers;

namespace ESFA.DC.Summarisation.Apps1920.Service
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1920);

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Apps1920.Service.JsonFiles.FundingTypes.json";
    }
}
