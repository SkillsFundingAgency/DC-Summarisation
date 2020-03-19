﻿using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.NCS.Model.Config;

namespace ESFA.DC.Summarisation.NCS.NCS2021.Service.Providers
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.NCS2021;

        protected internal override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.NCS.NCS2021.Service.JsonFiles.CollectionPeriods.json";
    }
}
