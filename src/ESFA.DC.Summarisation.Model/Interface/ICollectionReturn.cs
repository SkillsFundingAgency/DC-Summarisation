using System;
using System.Collections.Generic;
using System.Text;

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
