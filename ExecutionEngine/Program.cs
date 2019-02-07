using TradingCommon.QueueProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCommon;
using TradingCommon.DataModule;
using System.Configuration;

namespace ExecutionEngine
{
    class Program
    {

        static void Main(string[] args)
        {
            string queuepath = ConfigurationManager.AppSettings["OrderQueuePath"];
            OrderDataProcessor _ordprocssor = new OrderDataProcessor(new SqliteDbProvider());
            IQueueEngine queueEngine = new MsQueueEngine();
            OrderQueueProcessor queueProcessor = new OrderQueueProcessor(queueEngine);
            queueProcessor.createOrderQueue(@".\Private$\"+ queuepath);
            queueProcessor.SetupMessageReciever(typeof(Order));
            queueProcessor.ReadOrderQueue();         
            Console.ReadLine();
            queueProcessor.DisposeQueue();
        }
    }
}
