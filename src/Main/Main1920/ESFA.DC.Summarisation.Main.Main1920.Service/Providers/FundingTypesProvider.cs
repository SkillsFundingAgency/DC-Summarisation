﻿using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Main.Main1920.Service.Providers;
using ESFA.DC.Summarisation.Service.Model.Config;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.ILR1920;

        protected internal override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Main.Main1920.Service.JsonFiles.FundingTypes.json";
    }
}
