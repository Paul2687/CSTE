using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCommon;
using TradingCommon.DataModule;
using System.Data;
using System.Configuration;
using System.IO;

namespace ExecutionEngine
{
   public class OrderDataProcessor
    {
        private IDataBaseProvider _OrderDatabase;
        private string _ConnString;
        public OrderDataProcessor(IDataBaseProvider idbprovider)
        {
            this._OrderDatabase = idbprovider;
            string dbpath = ConfigurationManager.AppSettings["Sqlitepath"];
            this._ConnString = @"Data Source=" + dbpath + ";Version=3;";
        }
        public int InsertOrder(Order order)
        {
            string sql = "Insert into tbl_orders(StockID,RequestID,OrderDt,OrderSide,Quantity,AvlQuantity,Price) values(@stock,@request,@orddt,@ordSide,@quantity,@avlquantity,@price) ; SELECT last_insert_rowid();";
            try
            {
                using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                {
                    using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                    {
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@stock", order.StockId));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@request", order.RequestId));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@orddt", order.SubmittedDt));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@ordSide", (int)order.OrderSide));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@quantity", order.Quantity));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@avlquantity", order.Quantity));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@price", order.Price));
                         object obj = command.ExecuteScalar();
                         int id = Convert.ToInt32(obj);
                         return id;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public int InsertExecutions(ExecutedOrder exeorder)
        {
            string sql = "Insert into tbl_trades(StockID,BuyRequestID,SellRequestID,ExeQuantity,ExePrice) values(@stock,@buyrequest,@sellrequest,@exequantity,@exeprice) ; SELECT last_insert_rowid();";
            try
            {
                using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                {
                    using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                    {
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@stock", exeorder.StockID));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@buyrequest", exeorder.BuyRequestID));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@sellrequest", exeorder.SellRequestID));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@exequantity", exeorder.ExecutionQuantity));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@exeprice", exeorder.ExecutionPrice));
                        object obj = command.ExecuteScalar();
                        int id = Convert.ToInt32(obj);
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public bool UpdateExecutedQuantity(long buyorderid,long sellorderid,long avlqnty)
        {
            string sql = "update tbl_orders set AvlQuantity=AvlQuantity-@avlquantity where ID=@buyorderid or ID=@sellorderid";
            try
            {
                using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                {
                    using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                    {
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@avlquantity", avlqnty));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@buyorderid", buyorderid));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@sellorderid", sellorderid));
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public IList<Order> FetchAvailableOrders(Order recvdord)
        {
           var orderlst = new List<Order>();
            string sql = "";
            int direction = 0;
            if(recvdord.OrderSide==OrderDirection.BUY)
            {
                sql = "Select ID,RequestID,Avlquantity,Price from tbl_orders where StockID=@stockid and orderside=@ordside and Avlquantity>0 and price<=@price order by price,orderdt asc ";
                direction = (int)OrderDirection.SELL;
            }
            else
            {
                sql = "Select ID,RequestID,Avlquantity,Price from tbl_orders where StockID=@stockid and orderside=@ordside and Avlquantity>0 and price>=@price order by price desc,orderdt asc ";
                direction = (int)OrderDirection.BUY;
            }
            try
            {
                using (IDbConnection conn = _OrderDatabase.CreateConnection(_ConnString))
                {
                    using (IDbCommand command = _OrderDatabase.CreateCommand(sql, conn))
                    {
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@stockid", recvdord.StockId));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@ordside", direction));
                        command.Parameters.Add(_OrderDatabase.PrepareParameter("@price", recvdord.Price));
                        IDataReader r = command.ExecuteReader();
                        while (r.Read())
                        {
                            orderlst.Add(new Order { ID = Convert.ToInt32(r["ID"]), RequestId = Convert.ToString(r["RequestID"]), AvailableQuantity = Convert.ToInt32(r["Avlquantity"]), Price= Convert.ToDecimal(r["Price"]) });
                        }
                        return orderlst;
                    }
                }
            }
            catch (Exception ex)
            {
                return orderlst;
            }
            return orderlst;
        }
    }
}
