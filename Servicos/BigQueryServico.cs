using Google.Cloud.BigQuery.V2;
using Rinoceronte.Db;
using Rinoceronte.Interfaces;
using Rinoceronte.Pages;
using System.Text.Json;

namespace Rinoceronte.Servicos
{
    public class BigQueryServico : IBigQueryServico
    {
        private readonly DbBigQuery _bigqueryDB;

        private readonly ILogger<IndexModel> _logger;

        public BigQueryServico(DbBigQuery bigqueryDB, ILogger<IndexModel> logger)
        {
            _bigqueryDB = bigqueryDB;
            _logger = logger;
        }
        
        public async Task<BigQueryResults> ExecutarBuscarGenerica(string query)
        {
            try
            {
                BigQueryJob job = _bigqueryDB.bigqueryClient.CreateQueryJob(
                              sql: query,
                              parameters: null,
                              options: new QueryOptions { UseQueryCache = false });
                job = job.PollUntilCompleted().ThrowOnAnyError();
                var linhas = await _bigqueryDB.bigqueryClient.GetQueryResultsAsync(job.Reference);

                return linhas;

            }
            catch (Exception ex)
            {

                _logger.LogError("Erro ao executar consulta: " + ex.Message);
                return null!;
            }
        }
    }
}
