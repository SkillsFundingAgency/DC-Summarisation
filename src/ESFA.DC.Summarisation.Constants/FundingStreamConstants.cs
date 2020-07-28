using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Constants
{
    public static class FundingStreamConstants
    {
        public const string Levy1799 = "LEVY1799";

        public const string NonLevy2019 = "NONLEVY2019";

        public const string NonLevy_1618NLAP2018 = "16-18NLAP2018";

        public const string NonLevy_ANLAP2018 = "ANLAP2018";

        public const string NonLevy_APPS2021 = "APPS2021";

        public const string NonLevy_APPS1920 = "APPS1920";

        public const string NonLevy_APPS1819 = "APPS1819";

        public const string Traineeship_1618TRN1920 = "16-18TRN1920";

        public const string Traineeship_AEBC19TRN1920 = "AEBC-19TRN1920";

        public const string Traineeship_AEB19TRN1920 = "AEB-19TRN1920";

        public const string AdultSkills_AEBCASCL1920 = "AEBC-ASCL1920";

        public const string AdultSkills_AEBAS1920 = "AEB-AS1920";

        public const string LoansBursary_ALLB1920 = "ALLB1920";

        public const string LoansBursary_ALLBC1920 = "ALLBC1920";

        public const string ESF1420 = "ESF1420";

        public const string LineType_EAS = "EAS";

        public static List<string> FundingStreams = new List<string>()
        {
            Traineeship_1618TRN1920,
            Traineeship_AEBC19TRN1920,
            AdultSkills_AEBCASCL1920,
            Traineeship_AEB19TRN1920,
            AdultSkills_AEBAS1920,
            LoansBursary_ALLB1920,
            LoansBursary_ALLBC1920,
            Levy1799,
            NonLevy2019,
            NonLevy_1618NLAP2018,
            NonLevy_ANLAP2018,
            NonLevy_APPS1920,
            NonLevy_APPS1819,
        };

        public static List<string> ESFFundingStreams = new List<string>()
        {
            ESF1420,
        };
    }
}