using iBank.Operator.Desktop.Extensions;
using iBank.Operator.Desktop.Files;

using System.Data.SqlClient;
using System.Windows;

namespace iBank.Operator.Desktop.Core
{
    public static class ConnectionManager
    {

#if KEEPSQLOPEN
        public static SqlConnection MSSQL_Connection { get; set; } = new ConfigJsonFile().GetMainSqlConnection();

        static ConnectionManager()
        {
            // Нам не нужно держать соединение постоянно открытым, открытие\закрытие очнь быстрое
            if (!MSSQL_Connection.TryOpen())
                MessageBox.Show("Не удалось подключиться к БД!", "Ошибка!");
            MSSQL_Connection.StateChange += MSSQL_Connection_StateChange;
        }

        private static void MSSQL_Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Closed)
            {
            reconnect:
                if (!MSSQL_Connection.TryOpen())
                {
                    switch(MessageBox.Show("Не удалось переподключиться к БД!", "Ошибка!", MessageBoxButton.YesNo))
                    {
                        case MessageBoxResult.Yes:
                            goto reconnect;
                    }
                }
            }
        }
#else
        public static SqlConnection MSSQL_Connection => new ConfigJsonFile().GetMainSqlConnection();
#endif
    }
}