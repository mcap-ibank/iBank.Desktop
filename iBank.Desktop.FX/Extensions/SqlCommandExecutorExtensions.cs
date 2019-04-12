using iBank.Core;

using System;

namespace iBank.Desktop.Extensions
{
    public static class SqlCommandExecutorExtensions
    {
        public static bool TryExecuteScalar(this SqlCommandExecutor comm, out object? result)
        {
            try
            {
                result = comm.Command.ExecuteScalar();
                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowException(ex);
                result = default;
                return false;
            }
        }

        public static bool TryExecuteNonQuery(this SqlCommandExecutor comm, out int rowsAffected)
        {
            try
            {
                rowsAffected = comm.Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)// when(ex is SqlException)
            {
                Utils.ShowException(ex);
                rowsAffected = 0;
                return false;
            }
        }
    }
}
