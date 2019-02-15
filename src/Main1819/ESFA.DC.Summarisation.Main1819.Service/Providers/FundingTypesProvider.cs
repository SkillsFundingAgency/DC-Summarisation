﻿using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;

namespace ESFA.DC.Summarisation.Main1819.Service.Providers
{
    public class FundingTypesProvider : AbstractStaticDataProvider<FundingType>
    {
        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Main1819.Service.JsonFiles.FundingTypes.json";

        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }
    }
}