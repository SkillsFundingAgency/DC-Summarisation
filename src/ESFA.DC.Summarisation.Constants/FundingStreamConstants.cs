using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Constants
{
    public static class FundingStreamConstants
    {
        public const string Levy1799 = "LEVY1799";

        public const string NonLevy2019 = "NONLEVY2019";

        public const string NonLevy_1618NLAP2018 = "16-18NLAP2018";

        public const string NonLevy_ANLAP2018 = "ANLAP2018";

        public const string NonLevy_APPS1920 = "APPS1920";

        public const string NonLevy_APPS1819 = "APPS1819";

        public static List<string> FundingStreams = new List<string>()
        {
            "16-18TRN1920",
            "AEBC-19TRN1920",
            "AEBC-ASCL1920",
            "AEB-19TRN1920",
            "AEB-AS1920",
            "ALLB1920",
            "ALLBC1920",
            "LEVY1799",
            "NONLEVY2019",
            "16-18NLAP2018",
            "ANLAP2018",
            "APPS1920",
            "APPS1819",
            "ESF1420",
        };
    }
}
