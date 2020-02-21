using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Mapper
{
    public class EventLogMapper : IEventLogMapper
    {
        public EventLog Map(CollectionReturn collectionReturn)
        {
            return new EventLog
            {
                CollectionType = collectionReturn.CollectionType,
                CollectionReturnCode = collectionReturn.CollectionReturnCode,
                DateTime = collectionReturn.DateTime
            };
        }
    }
}
