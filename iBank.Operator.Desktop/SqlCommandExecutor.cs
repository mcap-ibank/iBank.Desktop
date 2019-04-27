using iBank.Operator.Desktop.Core;

using System;
using System.Data;
using System.Data.SqlClient;

namespace iBank.Operator.Desktop
{
    public class SqlCommandExecutor : IDisposable
    {
        private SqlConnection Connection { get; } = ConnectionManager.MSSQL_Connection;
        public SqlCommand Command { get; }

        public SqlCommandExecutor(string sql, int timeout = 3)
        {
#if !KEEPSQLOPEN
            if (Connection.State == ConnectionState.Closed)
                Connection?.Open();
            else
                throw new Exception("Какого хуя подключение открыто, используй using!");
#endif
            Command = new SqlCommand(sql, Connection)
            {
                CommandTimeout = timeout
            };
        }

        public SqlDataReader ExecuteReader() => Command.ExecuteReader();

        public object ExecuteScalar() => Command.ExecuteScalar();

        public bool TryExecuteScalar(out object result, out Exception exception)
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