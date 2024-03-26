using Rinoceronte.Db;
using Rinoceronte.Interfaces;
using Rinoceronte.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os servi�os ao cont�iner.
builder.Services.AddRazorPages();
builder.Services.AddScoped<DbMySql, DbMySql>();
builder.Services.AddScoped<DbBigQuery, DbBigQuery>();
builder.Services.AddScoped<IBigQueryServico, BigQueryServico>();

// Configura��o da configura��o (Configuration).
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
