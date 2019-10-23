using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Stateless.Context
{
    public class JobContextMessageSummarisationContext : ISummarisationMessage
    {
        private const string _collectionType = "CollectionType";

        private const string _collectionReturnCode = "CollectionReturnCode";

        private const string _ukprn = "UkPrn";

        private const string _processType = "ProcessType";

        private const string _collectionYear = "CollectionYear";

        private const string _collectionMonth = "ReturnPeriod";

        private const string _reRunSummarisation = "Re-Run";

        private readonly JobContextMessage _jobContextMessage;

        public JobContextMessageSummarisationContext(JobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public string CollectionType
        {
            get => GetKeyValue(_collectionType);
        }

        public string CollectionReturnCode
        {
            get => GetKeyValue(_collectionReturnCode);
        }

        public int? Ukprn
        {
            get
            {
                int.TryParse(_jobContextMessage.KeyValuePairs[_ukprn].ToString(), out int result);

                return result;
            }
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
            get => GetKeyValue(_processType);
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

        public string GetKeyValue(string key)
        {
            string returnValue = string.Empty;

            if (SummarisationTypes.Any(item => item.Equals(SummarisationTypeConstants.Main1920_FM25, StringComparison.OrdinalIgnoreCase)
                        || item.Equals(SummarisationTypeConstants.Main1920_ALB, StringComparison.OrdinalIgnoreCase)
                        || item.Equals(SummarisationTypeConstants.Main1920_TBL, StringComparison.OrdinalIgnoreCase)
                        || item.Equals(SummarisationTypeConstants.Main1920_EAS, StringComparison.OrdinalIgnoreCase)
                        || item.Equals(SummarisationTypeConstants.Main1920_FM35, StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}DC"].ToString();
            }
            else if (SummarisationTypes.Any(item => item.Equals(SummarisationTypeConstants.ESF_ILRData, StringComparison.OrdinalIgnoreCase)
                   || item.Equals(SummarisationTypeConstants.ESF_SuppData, StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}ESF"].ToString();
            }
            else if (SummarisationTypes.Any(item => item.Equals(SummarisationTypeConstants.Apps1920_Levy, StringComparison.OrdinalIgnoreCase)
                   || item.Equals(SummarisationTypeConstants.Apps1920_NonLevy, StringComparison.OrdinalIgnoreCase)
                   || item.Equals(SummarisationTypeConstants.Apps1920_EAS, StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}App"].ToString();
            }

            return returnValue;
        }
    }
}
