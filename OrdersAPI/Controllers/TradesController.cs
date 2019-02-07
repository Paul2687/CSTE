using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TradingCommon;
using TradingCommon.DataModule;

namespace OrdersAPI.Controllers
{
    public class TradesController : ApiController
    {
        private StockEntity stc;

        public TradesController()
        {
            this.stc = new StockEntity(new SqliteDbProvider());
        }
        public IEnumerable<ExecutedOrder> GetAllTrades()
        {

            return new List<ExecutedOrder>();
        }
        
        public IHttpActionResult GetTrades(int id)
        {
            if (StockEntity.Stocklist.Any(a => a.StockID == id))
            {
                var trades = stc.GetExecutedOrders(id);
                if (trades == null || trades.Count() == 0)
                {
                   return NotFound();
                }
                return Ok(trades);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("api/submit")]
        public IHttpActionResult Submit(Order recievedord)
        {
            if (StockEntity.Stocklist.Any(a => a.StockID == recievedord.StockId))
            {
                string queuepath = ConfigurationManager.AppSettings["OrderQueuePath"];
                recievedord.SubmittedDt = DateTime.Now;
                recievedord.AvailableQuantity = recievedord.Quantity;
                recievedord.RequestId = "R" + stc.GetNextRequestNumber();
                using (MessageQueue messageQueue = new MessageQueue(@".\Private$\" + queuepath))
                {
                    messageQueue.Send(recievedord, "IDG");
                }
                return Ok(recievedord);
            }
            else
            {
                return BadRequest("Invalid Ticker");
            }
        }
    }
}

