using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace TradingCommon.DataModule
{
  public  class SqliteDbProvider : IDataBaseProvider
    {
        public IDbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection dbconnection)
        {
            SQLiteCommand command = (SQLiteCommand)CreateCommand();
            command.CommandText = commandText;
            command.Connection = (SQLiteConnection)dbconnection;
            command.CommandType = CommandType.Text;
            return command;
        }

        public IDbConnection CreateConnection(string ConnectionString)
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public IDataParameter PrepareParameter(string paramname, object paramvalue)
        {
            return new SQLiteParameter(paramname, paramvalue);
        }
    }
}
