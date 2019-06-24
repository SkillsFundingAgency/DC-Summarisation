using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EF.Console.DesignTime
{
    public class ReferenceDataCandidateNamingService : CandidateNamingService
    {
        public override string GenerateCandidateIdentifier(DatabaseTable originalTable)
        {
            return originalTable.Name;
        }

        public override string GenerateCandidateIdentifier(DatabaseColumn originalColumn)
        {
            return originalColumn.Name;
        }
    }
}
