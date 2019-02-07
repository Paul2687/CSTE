using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TradingCommon.QueueProcessor
{
    public interface IQueueEngine
    {
        bool IsQueueExist(string queueaddress);

        MessageQueue CreateMsgQueue(string queueaddress);
        
    }
}
