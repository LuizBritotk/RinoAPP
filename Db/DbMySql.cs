using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ILogger = Serilog.ILogger; 

namespace Rinoceronte.Db
{
    public class DbMySql : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MySqlConnection Connection { get; private set; }

        public DbMySql(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            try
            {
                string connectionString = ObterConnectionStringMySQL();

                Connection = new MySqlConnection(connectionString);
                Connection.Open();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Erro ao conectar ao MySQL: {ErrorMessage}");
                Dispose();
                throw;
            }
        }

        public async Task<(bool, string, TimeSpan)> ExecutarQueryMySQL()
        {
            bool connected = false;
            string maxCodigo = "";
            TimeSpan executionTime = TimeSpan.Zero;

            try
            {
                await using (var connection = Connection)
                {
                    string sql = "SELECT * FROM DBPORTAL.RH_ADIANTAMENTO";
                    _logger.Information("Executando consulta MySQL: {Query}", sql);

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        var result = await command.ExecuteScalarAsync();
                        stopwatch.Stop();
                        executionTime = stopwatch.Elapsed;

                        maxCodigo = result != null ? result.ToString()! : string.Empty;
                    }

                    connected = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Erro ao executar consulta MySQL: {ErrorMessage}");
            }

            return (connected, maxCodigo, executionTime);
        }

        public void Dispose()
        {
            if (Connection != null && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }

        private string ObterConnectionStringMySQL()
        {
            string server = _configuration["Databases:MySQL:Server"];
            int port = _configuration.GetValue<int>("Databases:MySQL:Port");
            string username = _configuration["Databases:MySQL:Username"];
            string password = _configuration["Databases:MySQL:Password"];
            string database = _configuration["Databases:MySQL:Database"];

            string connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password}";
            return connectionString;
        }
    }
}
