using System.Collections.Generic;

namespace FundingTypesGenerator.Model.Json
{
    public class FundLine
    {
        public string Fundline { get; set; }
        public string LineType { get; set; }
        public bool UseAttributes { get; set; }
        public List<string> Attributes { get; set; }
    }
}