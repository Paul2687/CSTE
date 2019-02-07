using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using TradingCommon;
using TradingCommon.QueueProcessor;

namespace ExecutionEngine
{
    class OrderQueueProcessor
    {
        private IQueueEngine IorderQueueEngine;
        private MessageQueue orderMsgQueue;
        private OrderProcessor ordPrcr;

        public OrderQueueProcessor(IQueueEngine orderqueue)
        {
            this.IorderQueueEngine = orderqueue;
            ordPrcr = new OrderProcessor();
        }

        internal bool createOrderQueue( string queueaddress)
        {
            if(IorderQueueEngine!=null)
            {
                if (!IorderQueueEngine.IsQueueExist(queueaddress))
                {
                    orderMsgQueue = IorderQueueEngine.CreateMsgQueue(queueaddress);
                                        
                }
                else
                {
                    orderMsgQueue = new MessageQueue(queueaddress);
                }
                return true;
            }
            return false;
        }

        internal void SetupMessageReciever( Type MessgaeBodytype)
        {
            if(orderMsgQueue!=null)
            {
                orderMsgQueue.Formatter = new XmlMessageFormatter(new Type[]
                                            {typeof(Order)});
                orderMsgQueue.ReceiveCompleted += OrderMsgQueue_ReceiveCompleted;
            }

        }

        internal bool ReadOrderQueue()
        {
            if(orderMsgQueue!=null && orderMsgQueue.CanRead)
            {
                orderMsgQueue.BeginReceive();
                return true;
            }

            return false;
        }

        internal void DisposeQueue()
        {
            orderMsgQueue.Dispose();
        }

        internal void OrderMsgQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {


            if (e.Message != null)
            {
                var message = e.Message;
                var order = (Order)message.Body;
                Console.WriteLine("Order Recieved. Request ID # " + order.RequestId);
                ordPrcr.ProcessOrder(order);
                Console.WriteLine("Order Processed. Request ID # " + order.RequestId);
            }
            

            orderMsgQueue.BeginReceive();
        }

        internal void SendMsgtoQueue(object msgbody)
        {
            if(msgbody!=null && orderMsgQueue!=null)
            {
                orderMsgQueue.Send(msgbody);
            }
        }
    }
}
