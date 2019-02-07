using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using TradingCommon;
using TradingCommon.DataModule;

namespace OrdersAPI.Models
{
    public class StockEntity
    {
        private IDataBaseProvider _OrderDatabase;
        private string _ConnString;
        private readonly object balanceLock = new object();
        public static IList<Stock> Stocklist;
        private static int _requestCount;

        public StockEntity(IDataBaseProvider idbprovider)
        {
            string dbpath = ConfigurationManager.AppSettings["Sqlitepath"];
            this._OrderDatabase = idbprovider;
            this._ConnString = @"Data Source="+ dbpath + ";Version=3;";
        }
        public IList<Stock> FetchStocks()
        {
            if (Stocklist == null)
            {
                Stocklist = new List<Stock>();

                string sql = "Select ID,StockID from tbl_StockNames;";
                try
                {
                    using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                    {
                        using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                        {
                            IDataReader r = command.ExecuteReader();
                            while (r.Read())
                            {
                                Stocklist.Add(new Stock { StockID = Convert.ToInt32(r["ID"]), StockName = Convert.ToString(r["StockId"]) });
                            }
                            return Stocklist;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Stocklist;
                }
            }
            else
                return Stocklist;
        }
        public int GetNextRequestNumber()
        {
            int requestnr = 0;
            if (_requestCount == 0)
            {
                string sql = "SELECT seq FROM 'sqlite_sequence' where name='tbl_orders';";
                try
                {
                    using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                    {
                        using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                        {
                            requestnr = Convert.ToInt32(command.ExecuteScalar()) + 1;
                            lock (balanceLock)
                            {
                                _requestCount = requestnr;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else
            {
                lock (balanceLock)
                {
                    _requestCount += 1;
                    requestnr = _requestCount;
                }
            }
            return requestnr;


        }

        public IList<ExecutedOrder> GetExecutedOrders(int Stockid)
        {
            List<ExecutedOrder> TradesList = new List<ExecutedOrder>();

            string sql = "Select * from tbl_trades where StockID=@stockid;";
            try
            {
                using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                {
                    using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                    {
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@stockid", Stockid));
                        IDataReader r = command.ExecuteReader();
                        while (r.Read())
                        {
                            TradesList.Add(new ExecutedOrder { BuyRequestID = Convert.ToString(r["BuyRequestID"]), SellRequestID = Convert.ToString(r["SellRequestID"]), ExecutionPrice = Convert.ToDecimal(r["ExeQuantity"]), ExecutionQuantity = Convert.ToInt64(r["ExePrice"]) });
                        }
                        return TradesList;
                    }
                }
            }
            catch
            {
                return TradesList;
            }
        }



    }
}