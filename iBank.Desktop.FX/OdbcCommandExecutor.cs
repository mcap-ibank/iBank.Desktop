using iBank.Core;

using System;
using System.Data;
using System.Data.Odbc;

namespace iBank.Desktop
{
    // Microsoft Access Database Engine 2010 Redistributable
    // https://www.microsoft.com/en-us/download/details.aspx?id=13255
    // MSAccess in DotNetCore
    // https://mrojas.ghost.io/msaccess-in-dotnetcore/
    public class OdbcCommandExecutor : IDisposable
    {
        private OdbcConnection Connection { get; } = new OdbcConnection(ConnectionManager.MSAccess_ConnectionString);
        public OdbcCommand Command { get; }

        public OdbcCommandExecutor(string sql, int timeout = 3)
        {
#if !KEEPSQLOPEN
            if (Connection.State == ConnectionState.Closed)
                Connection?.Open();
            else
                throw new Exception("Какого хуя подключение открыто, используй using!");
#endif
            Command = new OdbcCommand(sql, Connection) { CommandTimeout = timeout };
        }

        public OdbcDataReader ExecuteReader() => Command.ExecuteReader();

        public object ExecuteScalar() => Command.ExecuteScalar();

        public bool TryExecuteScalar(out object? result, out Exception exception)
        {
            try
            {
                result = Command.ExecuteScalar();
                exception = null;
                return true;
            }
            catch (Exception ex)// when(ex is SqlException)
            {
                exception = ex;
                result = default;
                return false;
            }
        }

        public bool TryExecuteNonQuery(out int rowsAffected, out Exception exception)
        {
            try
            {
                rowsAffected = Command.ExecuteNonQuery();
                exception = null;
                return true;
            }
            catch (Exception ex)// when(ex is SqlException)
            {
                exception = ex;
                rowsAffected = 0;
                return false;
            }
        }

        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open)
                Connection?.Close();
            else
                throw new Exception("Какого хуя подключение не закрыто, используй using!");
            Command?.Dispose();
        }
    }
}