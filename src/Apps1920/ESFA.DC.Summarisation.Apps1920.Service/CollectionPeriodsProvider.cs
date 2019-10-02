﻿using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Service.Providers;

namespace ESFA.DC.Summarisation.Apps1920.Service
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => nameof(Configuration.Enum.CollectionType.APPS);

        protected override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Apps1920.Service.JsonFiles.CollectionPeriods.json";
    }
}
