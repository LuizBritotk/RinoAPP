using Google.Apis.Bigquery.v2;
using Rinoceronte.Db;
using Rinoceronte.Interfaces;
using Rinoceronte.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<DbMySql, DbMySql>();
builder.Services.AddScoped<DbBigQuery, DbBigQuery>();

builder.Services.AddScoped<IBigQueryServico, BigQueryServico>();

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
