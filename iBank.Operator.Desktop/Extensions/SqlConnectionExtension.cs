using iBank.Operator.Desktop.Utils;

using System;
using System.Data.SqlClient;

namespace iBank.Operator.Desktop.Extensions
{
    public static class SqlConnectionExtension
    {
        public static bool TryOpen(this SqlConnection conn)
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception ex)
            {
                CommonUtils.ShowException(ex);
                return false;
            }
        }
    }
}