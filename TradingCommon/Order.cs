using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingCommon
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime SubmittedDt { get; set; }
        public string StockName { get; set; }
        public int StockId { get; set; }
        public long Quantity { get; set; }
        public long AvailableQuantity { get; set; }
        public Decimal Price { get; set; }
        public string RequestId { get; set; }
        public OrderDirection OrderSide { get; set; }
    }

    public enum OrderDirection
    {
        BUY =1,
        SELL=2
    }
}
