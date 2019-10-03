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

            if (SummarisationTypes.Any(item => item.Equals(Configuration.Enum.SummarisationType.Main1819_FM35.ToString(), StringComparison.OrdinalIgnoreCase)
                        || item.Equals(Configuration.Enum.SummarisationType.Main1819_FM25.ToString(), StringComparison.OrdinalIgnoreCase)
                        || item.Equals(Configuration.Enum.SummarisationType.Main1819_ALB.ToString(), StringComparison.OrdinalIgnoreCase)
                        || item.Equals(Configuration.Enum.SummarisationType.Main1819_TBL.ToString(), StringComparison.OrdinalIgnoreCase)
                        || item.Equals(Configuration.Enum.SummarisationType.Main1819_EAS.ToString(), StringComparison.OrdinalIgnoreCase)
                        || item.Equals(Configuration.Enum.SummarisationType.Main1920_FM35.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}DC"].ToString();
            }
            else if (SummarisationTypes.Any(item => item.Equals(Configuration.Enum.SummarisationType.ESF_ILRData.ToString(), StringComparison.OrdinalIgnoreCase)
                   || item.Equals(Configuration.Enum.SummarisationType.ESF_SuppData.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}ESF"].ToString();
            }
            else if (SummarisationTypes.Any(item => item.Equals(Configuration.Enum.SummarisationType.Apps1819_Levy.ToString(), StringComparison.OrdinalIgnoreCase)
                   || item.Equals(Configuration.Enum.SummarisationType.Apps1819_NonLevy.ToString(), StringComparison.OrdinalIgnoreCase)
                   || item.Equals(Configuration.Enum.SummarisationType.Apps1920_Levy.ToString(), StringComparison.OrdinalIgnoreCase)
                   || item.Equals(Configuration.Enum.SummarisationType.Apps1920_NonLevy.ToString(), StringComparison.OrdinalIgnoreCase)
                   || item.Equals(Configuration.Enum.SummarisationType.Apps1920_EAS.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                returnValue = _jobContextMessage.KeyValuePairs[$"{key}App"].ToString();
            }

            return returnValue;
        }
    }
}
