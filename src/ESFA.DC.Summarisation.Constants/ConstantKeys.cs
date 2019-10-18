using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Constants
{
    public static class ConstantKeys
    {
        public const string Apps1618NonLevyContractProcured = "16-18 Apprenticeship Non-Levy Contract (procured)";

        public const string Apps19plusNonLevyContractProcured = "19+ Apprenticeship Non-Levy Contract (procured)";

        public const string Levy1799 = "LEVY1799";

        public const string NonLevy2019 = "NONLEVY2019";

        public const string NonLevy_1618NLAP2018 = "16-18NLAP2018";

        public const string NonLevy_ANLAP2018 = "ANLAP2018";

        public const string NonLevy_APPS1920 = "APPS1920";

        public const int ContractType_Levy = 1;

        public const int ContractType_NonLevy = 2;

        public const string NonLevy_APPS1819 = "APPS1819";

        public const string LineType_EAS = "EAS";

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

        public const string ReRunSummarisation = "Re-Run";

        public static List<string> Levy1799_EASlines = new List<string>()
        {
            "Authorised Claims: 16-18 Levy Apprenticeships - Training",
            "Excess Learning Support: 16-18 Levy Apprenticeships - Provider",
            "Authorised Claims: 16-18 Levy Apprenticeships - Provider",
            "Authorised Claims: 16-18 Levy Apprenticeships - Employer",
            "Authorised Claims: 16-18 Levy Apprenticeships - Apprentice",
            "Authorised Claims: Adult Levy Apprenticeships - Training",
            "Excess Learning Support: Adult Levy Apprenticeships - Provider",
            "Authorised Claims: Adult Levy Apprenticeships - Provider",
            "Authorised Claims: Adult Levy Apprenticeships - Employer",
            "Authorised Claims: Adult Levy Apprenticeships - Apprentice",
        };

        //Collection Type Constants
        public const string CollectionType_ILR1819 = "ILR1819";
        public const string CollectionType_ILR1920 = "ILR1920";
        public const string CollectionType_ESF = "ESF";
        public const string CollectionType_Apps1819 = "Apps1819";
        public const string CollectionType_APPS = "APPS";

        //PeriodTypeCode Constants
        public const string PeriodTypeCode_CM = "CM";

        //ProcessType Constants
        public const string ProcessType_Fundline = "Fundline";
        public const string ProcessType_Deliverable = "Deliverable";
        public const string ProcessType_Payments = "Payments";

        //SummarisationType Constants
        public const string SummarisationType_ESF_SuppData = "ESF_SuppData";
        public const string SummarisationType_ESF_ILRData = "ESF_ILRData";
        public const string SummarisationType_Main1920_FM35 = "Main1920_FM35";
        public const string SummarisationType_Main1920_EAS = "Main1920_EAS";
        public const string SummarisationType_Main1920_FM25 = "Main1920_FM25"; 
        public const string SummarisationType_Main1920_ALB = "Main1920_ALB";
        public const string SummarisationType_Main1920_TBL = "Main1920_TBL";
        public const string SummarisationType_Apps1920_Levy = "Apps1920_Levy";
        public const string SummarisationType_Apps1920_NonLevy = "Apps1920_NonLevy";
        public const string SummarisationType_Apps1920_EAS = "Apps1920_EAS";
    }
}
