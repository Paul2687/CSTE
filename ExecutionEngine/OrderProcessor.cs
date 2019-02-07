using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCommon;
using TradingCommon.DataModule;

namespace ExecutionEngine
{
    public class OrderProcessor
    {
        public OrderProcessor()
        {
            orderdata = new OrderDataProcessor(new SqliteDbProvider());
        }
        private OrderDataProcessor orderdata;
        public bool ProcessOrder(Order order)
        {
            if(order!=null && order.Quantity>0)
            {
                var result = orderdata.InsertOrder(order);
                if(result>0)
                {
                    order.ID = result;
                  IList<Order> Unfilledorders=  orderdata.FetchAvailableOrders(order);
                    if(Unfilledorders!=null && Unfilledorders.Count>0)
                    {
                        ExecutedOrder exord = new ExecutedOrder();
                        exord.StockID = order.StockId;
                        foreach(Order uforder in Unfilledorders)
                        {
                            
                            if (order.OrderSide == OrderDirection.BUY)
                            {
                                exord.BuyRequestID = order.RequestId;
                                exord.SellRequestID = uforder.RequestId;
                                exord.ExecutionPrice = uforder.Price;
                            }
                            else
                            {
                                exord.BuyRequestID = uforder.RequestId;
                                exord.SellRequestID = order.RequestId;
                                exord.ExecutionPrice = order.Price;
                            }

                            if (uforder.AvailableQuantity>=order.AvailableQuantity)
                            {
                               exord.ExecutionQuantity = order.AvailableQuantity;
                               
                            }
                            else
                            {
                                exord.ExecutionQuantity = uforder.AvailableQuantity;
                            }
                             result= orderdata.InsertExecutions(exord);
                            if (result > 0)
                            {
                                orderdata.UpdateExecutedQuantity(order.ID, uforder.ID, exord.ExecutionQuantity);
                                order.AvailableQuantity -= exord.ExecutionQuantity;
                            }
                            else
                            {
                                Console.WriteLine("Error while Executing the trade");
                                break;
                            }
                            if(order.AvailableQuantity==0)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error while processing the order");
                    return false;
                }
            }
            return false;
        }

        public bool ExecuteTrade()
        {
            return false;
        }
    }
}
