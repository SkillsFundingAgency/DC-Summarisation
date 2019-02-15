using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<string> ToCaseInsensitiveHashSet(this IEnumerable<string> source)
        {
            return new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
        }
    }
}
