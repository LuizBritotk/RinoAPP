using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Rinoceronte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string logsDirectory = Path.Combine("C:\\", "Logs");

            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            Log.Logger = new LoggerConfiguration()
                             // Define o nível mínimo de log para Debug
                             .MinimumLevel.Debug()
                             // Sobrescreve o nível de log para o namespace "Microsoft" para Information
                             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                             // Enriquece o log com informações de contexto
                             .Enrich.FromLogContext()
                             // Define a saída do log para o console
                             .WriteTo.Console()
                             // Define a saída do log para um arquivo com rotação diária
                             // e limite ilimitado de arquivos de log antigos
                             .WriteTo.File(GetLogFilePath(), rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
                             // Cria o logger
                            .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Erro ao iniciar o projeto!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static string GetLogFilePath()
        {
            string logsDirectory = Path.Combine("C:\\", "Logs");
            string logFileName = $"logfile-{DateTime.Now:yyyy-MM-dd}.txt";

            return Path.Combine(logsDirectory, logFileName);
        }
    }
}
