using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ESFA.DC.Summarisation.Stateless.Context
{
    public class JobContextMessageSummarisationContext : ISummarisationMessage
    {
        private const string _collectionType = "CollectionType";

        private const string _collectionReturnCode = "CollectionReturnCode";

        private const string _ukprn = "UkPrn";

        private const string _processType = "ProcessType";

        private const string _collectionYear = "CollectionYear";

        private const string _collectionMonth = "CollectionMonth";

        private const string _reRunSummarisation = "Re-Run";

        private readonly JobContextMessage _jobContextMessage;

        public JobContextMessageSummarisationContext(JobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public string CollectionType
        {
            get => _jobContextMessage.KeyValuePairs[_collectionType].ToString();
        }

        public string CollectionReturnCode
        {
            get => _jobContextMessage.KeyValuePairs[_collectionReturnCode].ToString();
        }

        public string Ukprn
        {
            get => _jobContextMessage.KeyValuePairs[_ukprn].ToString();
        }

        public IEnumerable<string> SummarisationTypes
        {
            get
            {
                return _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(t => t.Tasks);
            }
        }

        public string ProcessType
        {
            get => _jobContextMessage.KeyValuePairs[_processType].ToString();
        }

        public int CollectionYear
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[_collectionYear].ToString());
        }

        public int CollectionMonth
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[_collectionMonth].ToString());
        }

        public bool RerunSummarisation
        {
            get
            {
                return SummarisationTypes.Any(x => x.Equals(_reRunSummarisation, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
