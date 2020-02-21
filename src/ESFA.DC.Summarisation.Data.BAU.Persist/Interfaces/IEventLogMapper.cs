using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces
{
    public interface IEventLogMapper
    {
        EventLog Map(CollectionReturn collectionReturn);
    }
}
