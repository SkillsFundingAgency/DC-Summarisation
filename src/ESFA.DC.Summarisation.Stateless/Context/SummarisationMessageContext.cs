using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Stateless.Context
{
        
    public class SummarisationMessageContext : ISummarisationMessageContext
    {
        private const string _collectionType = "CollectionType";

        private const string _collectionReturnCode = "CollectionReturnCode";

        private readonly JobContextMessage _jobContextMessage;
                
        public SummarisationMessageContext(JobContextMessage jobContextMessage)
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

        public IEnumerable<string> FundModels
        {
            get
            {
                return _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(t => t.Tasks);
            }
        }


    }
}
