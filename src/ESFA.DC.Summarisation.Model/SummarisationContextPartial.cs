using System.Linq;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Model
{
    public partial class SummarisationContext : ISummarisationContext
    {
        IQueryable<SummarisedActual> ISummarisationContext.SummarisedActuals => SummarisedActuals;

        IQueryable<CollectionReturn> ISummarisationContext.CollectionReturns => CollectionReturns;

        IQueryable<ESF_FundingData> ISummarisationContext.ESF_FundingDatas => ESF_FundingDatas;

        public virtual DbSet<SummarisedActualDto> SummarisedActualsDto { get; set; }
    }
}
