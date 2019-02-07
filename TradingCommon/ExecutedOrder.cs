using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingCommon
{
    public class ExecutedOrder
    {
        public string BuyRequestID { get; set; }
        public string SellRequestID { get; set; }
        public int StockID { get; set; }
        public long ExecutionQuantity { get; set; }
        public decimal ExecutionPrice { get; set; }
    }
}
