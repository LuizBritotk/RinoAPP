using Google.Cloud.BigQuery.V2;

namespace Rinoceronte.Interfaces
{
    public interface IBigQueryServico
    {
        Task<BigQueryResults> ExecutarBuscarGenerica(string query);
    }
}
