using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

namespace Rinoceronte.Db
{
    public class DbBigQuery
    {    
        public BigQueryClient bigqueryClient { get; set; }   

        public DbBigQuery()
        {
            // Cria as credenciais do firebase 1 com o arquivo JSON
            string filepath1 = Path.Combine(AppContext.BaseDirectory + "/projeto_lake_lojao.json");
            //Environment.SetEnvironmentVariable("GOOGLE_CLOUD_PROJECT", filepath1);
            var googleCredential = GoogleCredential.FromFile(filepath1);

            // Cria a conexão com banco Firestore Data Base
            string projectId1 = "projetodatalakevab";
            //bigqueryClient = BigQueryClient.Create(projectId1);

            bigqueryClient = BigQueryClient.Create(projectId1, googleCredential);

        }
    }
}
