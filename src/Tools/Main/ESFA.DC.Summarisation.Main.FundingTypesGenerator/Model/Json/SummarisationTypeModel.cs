using System.Collections.Generic;

namespace FundingTypesGenerator.Model.Json
{
    public class SummarisationTypeModel
    {
        public string SummarisationType { get; set; }
        public List<FundingStream> FundingStreams { get; set; }
    }
}