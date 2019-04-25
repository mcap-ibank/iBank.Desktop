using System;
using System.Data.SqlClient;

namespace iBank.Desktop.Extensions
{
    public static class SqlCommandExtension
    {
        public static bool TryExecuteScalar(this SqlCommand sqlcmd, out object? result)
        {
            try
            {
                result = sqlcmd.ExecuteScalar();
                return true;
            }
            catch (Exception ex)// when(ex is SqlException)
            {
                Utils.ShowException(ex);
                result = null;
                return false;
            }
        }
        public static bool TryExecuteNonQuery(this SqlCommand sqlcmd, out int rowsAffected)
        {
            try
            {
                rowsAffected = sqlcmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)// when(ex is SqlException)
            {
                Utils.ShowException(ex);
                rowsAffected = -1;
                return false;
            }
        }
    }
}