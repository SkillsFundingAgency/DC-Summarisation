using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFProvider : ILearningDeliveryProvider
    {
        private readonly IESF_DataStoreEntities _esf;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_ALB);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public ESFProvider(IESF_DataStoreEntities esf)
        {
            _esf = esf;
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
