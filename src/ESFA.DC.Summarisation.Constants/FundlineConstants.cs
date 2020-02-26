using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.Summarisation.Constants
{
    public static class FundlineConstants
    {
        public const string Apps1618NonLevyContractProcured = "16-18 Apprenticeship Non-Levy Contract (procured)";

        public const string Apps19plusNonLevyContractProcured = "19+ Apprenticeship Non-Levy Contract (procured)";

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

        public static List<string> NonLevy2019_EASlines = new List<string>()
        {
             "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Training",
             "Excess Learning Support: 16-18 Apprenticeship (Employer on App Service) Non-Levy",
             "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Provider",
             "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Employer",
             "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Apprentice",
             "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Training",
             "Excess Learning Support: 19+ Apprenticeship (Employer on App Service) Non-Levy",
             "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Provider",
             "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Employer",
             "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Apprentice",
        };

        public static List<string> EasLines_Levy_NonLevy2019 = Levy1799_EASlines.Union(NonLevy2019_EASlines).ToList();
    }
}
