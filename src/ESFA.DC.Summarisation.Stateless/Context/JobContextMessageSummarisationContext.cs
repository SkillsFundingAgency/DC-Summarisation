using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;

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

        private readonly JobContextMessage _jobContextMessage;

        private List<string> _excludeTasks = new List<string> { ConstantKeys.ReRunSummarisation, ConstantKeys.PublishToBAU };

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

        private ICollection<string> SummarisationTasks
        {
            get
            {
                return _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(t => t.Tasks).ToList();
            }
        }

        public ICollection<string> SummarisationTypes
        {
            get
            {
                return SummarisationTasks.Where(w => !_excludeTasks.Any(e => w.Equals(e, StringComparison.OrdinalIgnoreCase))).ToList();
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

        public bool PublishToBAU
        {
            get
            {
                return SummarisationTasks.Any(x => x.Equals(ConstantKeys.PublishToBAU, StringComparison.OrdinalIgnoreCase));
            }
        }

        public string GetKeyValue(string key)
        {
            try
            {
                string returnValue = string.Empty;

                if (SummarisationTypes.Any(item =>
                               item.Equals(SummarisationTypeConstants.Main1920_FM25, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main1920_ALB, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main1920_TBL, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main1920_EAS, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main1920_FM35, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main2021_FM25, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main2021_ALB, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main2021_TBL, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main2021_EAS, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.Main2021_FM35, StringComparison.OrdinalIgnoreCase)))
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
                else if (SummarisationTypes.Any(item => item.Equals(SummarisationTypeConstants.NCS1920_C, StringComparison.OrdinalIgnoreCase)
                            || item.Equals(SummarisationTypeConstants.NCS2021_C, StringComparison.OrdinalIgnoreCase)))
                {
                    returnValue = _jobContextMessage.KeyValuePairs[$"{key}NCS"].ToString();
                }
                else if (SummarisationTypes.Any(item => item.Equals(SummarisationTypeConstants.Generic, StringComparison.OrdinalIgnoreCase)))
                {
                    returnValue = _jobContextMessage.KeyValuePairs[$"{key}GC"].ToString();
                }

                return returnValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
