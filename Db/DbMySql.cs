using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.Json;

namespace Rinoceronte.Db
{
    public class DbMySql : IDisposable
    {
        private readonly ILogger<DbMySql> _logger;

        public MySqlConnection Connection { get; set; }

        public DbMySql(ILogger<DbMySql> logger) 
        {
            _logger = logger;

            try
            {
                string mySQLJsonFilePath = "appsettings.json";
                string mySQLJsonString = System.IO.File.ReadAllText(mySQLJsonFilePath);
                var mySQLJsonDocument = JsonDocument.Parse(mySQLJsonString);
                var mySQLDatabases = mySQLJsonDocument.RootElement.GetProperty("Databases");

                string mySQLConnectionString = BuildMySQLConnectionString(mySQLDatabases.GetProperty("MySQL"));

                Connection = new MySqlConnection(mySQLConnectionString);
                Connection.Open();
            }
            catch (Exception ex)
            {
                Dispose();
                throw new Exception("Erro ao conectar ao MySQL: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (Connection != null && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }

        private string BuildMySQLConnectionString(JsonElement mySQLInfo)
        {
            string server = mySQLInfo.GetProperty("Server").GetString()!;
            int port = mySQLInfo.GetProperty("Port").GetInt32();
            string username = mySQLInfo.GetProperty("Username").GetString()!;
            string password = mySQLInfo.GetProperty("Password").GetString()!;
            string database = mySQLInfo.GetProperty("Database").GetString()!;

            try
            {
                string mySQLConnectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password}";
                return mySQLConnectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao recuperar configurações: " + ex.Message);
                throw;
            }
        }
    }
}
