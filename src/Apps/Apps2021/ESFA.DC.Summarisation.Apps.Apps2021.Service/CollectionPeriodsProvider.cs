using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Apps.Apps2021.Service
{
    public class CollectionPeriodsProvider : AbstractSummarisationConfigProvider<CollectionPeriod>
    {
        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
            : base(jsonSerializationService)
        {
        }

        public override string CollectionType => CollectionTypeConstants.APPS2021;

        protected internal override Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        protected internal override string ReferenceDataFileName { get; } = "ESFA.DC.Summarisation.Apps.Apps2021.Service.JsonFiles.CollectionPeriods.json";
    }
}
