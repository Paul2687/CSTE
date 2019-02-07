using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace TradingCommon.QueueProcessor
{
   public class MsQueueEngine : IQueueEngine
    {
        public MessageQueue CreateMsgQueue(string queueaddress)
        {
            if (!string.IsNullOrEmpty(queueaddress))
                return MessageQueue.Create(queueaddress);
            else
                return null;
        }

        public bool IsQueueExist(string queueaddress)
        {
            if(!string.IsNullOrEmpty(queueaddress))
                return MessageQueue.Exists(queueaddress);
            else
                return false;
        }
    }
}
