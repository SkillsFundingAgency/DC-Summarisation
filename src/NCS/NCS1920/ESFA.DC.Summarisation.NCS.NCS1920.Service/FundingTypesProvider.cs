﻿using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.NCS.Model.Config;

namespace ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers
{
    public class FundingTypesProvider : AbstractSummarisationConfigProvider<FundingType>
    {
        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.NCS1920;

        protected internal override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.NCS.NCS1920.Service.JsonFiles.FundingTypes.json";
    }
}
