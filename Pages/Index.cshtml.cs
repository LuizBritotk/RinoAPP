using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rinoceronte.Db;
using Rinoceronte.Interfaces;
using System.Diagnostics;

namespace Rinoceronte.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DbMySql _db;
        private readonly IBigQueryServico _bigQueryServico;

        public IndexModel(ILogger<IndexModel> logger, IBigQueryServico bigQueryClient, DbMySql db)
        {
            _logger = logger;
            _bigQueryServico = bigQueryClient;
            _db = db;
            MySQLConnected = false;
            MySQLMaxCodigo = string.Empty;
            MySQLQueryExecutionTime = TimeSpan.Zero;
            BigQueryConnected = false;
            BigQueryMaxCodigo = string.Empty;
            BigQueryQueryExecutionTime = TimeSpan.Zero;

        }

        public bool MySQLConnected { get; private set; }
        public string MySQLMaxCodigo { get; private set; }
        public TimeSpan MySQLQueryExecutionTime { get; private set; }

        public bool BigQueryConnected { get; private set; }
        public string BigQueryMaxCodigo { get; private set; }
        public TimeSpan BigQueryQueryExecutionTime { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                (MySQLConnected, MySQLMaxCodigo, MySQLQueryExecutionTime) = await _db.ExecutarQueryMySQL();
                (BigQueryConnected, BigQueryMaxCodigo, BigQueryQueryExecutionTime) = await ExecutarQueryBigQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao executar consulta: " + ex.Message);
            }
        }

        private async Task<(bool, string, TimeSpan)> ExecutarQueryBigQuery()
        {
            bool connected = false;
            string maxCodigo = "";
            TimeSpan executionTime = TimeSpan.Zero;

            try
            {
                string query = "SELECT MAX(CODIGO) AS Codigo FROM projetodatalakevab.DW_VAB.glb_recursos";
                Stopwatch stopwatch = Stopwatch.StartNew();
                var results = await _bigQueryServico.ExecutarBuscarGenerica(query);
                foreach (BigQueryRow row in results)
                {
                    maxCodigo = row["Codigo"].ToString()!;
                    break;
                }
                stopwatch.Stop();
                executionTime = stopwatch.Elapsed;
                connected = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao executar consulta BigQuery: " + ex.Message);
            }

            return (connected, maxCodigo, executionTime);
        }
    }
}
