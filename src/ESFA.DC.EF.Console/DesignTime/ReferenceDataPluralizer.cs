using Microsoft.EntityFrameworkCore.Design;

namespace ESFA.DC.ReferenceData.EF.Console.DesignTime
{
    public class ReferenceDataPluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            return name.Pluralize() ?? name;
        }

        public string Singularize(string name)
        {
            return name.Singularize() ?? name;
        }
    }
}
