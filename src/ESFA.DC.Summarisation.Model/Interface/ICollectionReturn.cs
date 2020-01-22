using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Model.Interface
{
    public interface ICollectionReturn
    {
        int Id { get; set; }

        string CollectionType { get; set; }

        string CollectionReturnCode { get; set; }

        ICollection<SummarisedActual> SummarisedActuals { get; set; }
    }
}
