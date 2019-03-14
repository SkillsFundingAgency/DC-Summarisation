using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ISummarisationMessage
    {
        string CollectionType { get; }
        string CollectionReturnCode { get;}
    }
}
