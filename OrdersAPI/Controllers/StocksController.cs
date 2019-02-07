using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TradingCommon;
using TradingCommon.DataModule;

namespace OrdersAPI.Controllers
{
    public class StocksController : ApiController
    {
        private StockEntity stc;
        public StocksController()
        {
            this.stc = new StockEntity(new SqliteDbProvider());
        }
        public IEnumerable<Stock> GetAllStocks()
        {
           return stc.FetchStocks();
        }

    }
}
