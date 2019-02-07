using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingCommon.DataModule
{
  public interface IDataBaseProvider
    {
         
         IDbConnection CreateConnection(string ConnectionString);
         IDbCommand CreateCommand();
         IDbCommand CreateCommand(string commandText, IDbConnection connection);
         IDataParameter PrepareParameter(string paramname, object paramvalue);
    }
}
