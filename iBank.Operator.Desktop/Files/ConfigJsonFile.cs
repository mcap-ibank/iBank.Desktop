using iBank.Core;
using iBank.Core.Files;
using iBank.Core.Folders;

using PCLExt.FileStorage;

using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;

namespace iBank.Operator.Desktop.Files
{

    public class ConfigJsonFile : BaseConfigJsonFile
    {
        #region Main SQL Database

        private string _SQL_MachineName = "UNKNOWN";
        public string SQL_MachineName { get => _SQL_MachineName; set => SetValueIfChangedAndSave(ref _SQL_MachineName, value); }

        private string[] _SQL_Endpoints = new[] { "192.168.137.1" };
        public string[] SQL_Endpoints { get => _SQL_Endpoints; set => SetValueIfChangedAndSave(ref _SQL_Endpoints, value); }

        private ushort _SQL_Port = 1435;
        public ushort SQL_Port { get => _SQL_Port; set => SetValueIfChangedAndSave(ref _SQL_Port, value); }

        private string _SQL_Database = "UNKNOWN";
        public string SQL_Database { get => _SQL_Database; set => SetValueIfChangedAndSave(ref _SQL_Database, value); }

        private string _SQL_User = "UNKNOWN";
        public string SQL_User { get => _SQL_User; set => SetValueIfChangedAndSave(ref _SQL_User, value); }

        private string _SQL_Password = "UNKNOWN";
        public string SQL_Password { get => _SQL_Password; set => SetValueIfChangedAndSave(ref _SQL_Password, value); }

        private string _SQL_ExtraArgs = "Connection Timeout=3";
        public string SQL_ExtraArgs { get => _SQL_ExtraArgs; set => SetValueIfChangedAndSave(ref _SQL_ExtraArgs, value); }

        #endregion

        #region Bank Provider

        private string _Bank_Provider_MachineName = "UNKNOWN";
        public string Bank_Provider_MachineName { get => _Bank_Provider_MachineName; set => SetValueIfChangedAndSave(ref _Bank_Provider_MachineName, value); }

        private string[] _Bank_Provider_Endpoints = new string[] { "192.168.137.123" };
        public string[] Bank_Provider_Endpoints { get => _Bank_Provider_Endpoints; set => SetValueIfChangedAndSave(ref _Bank_Provider_Endpoints, value); }

        private ushort _Bank_Provider_Port = 15565;
        public ushort Bank_Provider_Port { get => _Bank_Provider_Port; set => SetValueIfChangedAndSave(ref _Bank_Provider_Port, value); }

        #endregion

        public ConfigJsonFile() : base(new ConfigFolder().CreateFile("iBank.Operator.Config.json", CreationCollisionOption.OpenIfExists)) { }

        public string GetMainSQLConnectionString()
        {
            string host = default;
            if (host == null && GetIPAddressByMachineName(SQL_MachineName) != null)
                host = SQL_MachineName;
            if (host == null)
                host = GetFirstValidIPAddress(SQL_Endpoints, SQL_Port)?.ToString();
            if (host == null)
                throw new Exception("SQL Сервер недоступен!");

            var builder = new DbConnectionStringBuilder
            {
                { "Data Source", $"{host},{SQL_Port}" },
                { "Initial Catalog", SQL_Database },
                { "User ID", SQL_User },
                { "Password", SQL_Password }
            };
            return $"{builder.ConnectionString}; {SQL_ExtraArgs}";
        }

        public SqlConnection GetMainSqlConnection() => new SqlConnection(GetMainSQLConnectionString());

        public BankProviderClient GetBankProviderClient()
        {
            IPAddress host = default;
            if (host == null)
                host = GetIPAddressByMachineName(Bank_Provider_MachineName);
            if (host == null)
                host = GetFirstValidIPAddress(Bank_Provider_Endpoints, Bank_Provider_Port);
            if (host == null)
                throw new Exception("Банк недоступен! Компьютер подключен в сеть?");

            return new BankProviderClient(host, Bank_Provider_Port);
        }
    }
}