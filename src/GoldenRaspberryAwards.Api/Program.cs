using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Data;
using GoldenRaspberryAwards.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection")
    ?? "Data Source=razzies.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ICsvLoaderService, CsvLoaderService>();
builder.Services.AddScoped<IProducerIntervalService, ProducerIntervalService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var loader = scope.ServiceProvider.GetRequiredService<ICsvLoaderService>();
    var csvPath = builder.Configuration.GetValue<string>("CsvPath") ?? "Movielist.csv";
    await loader.LoadIfEmptyAsync(csvPath);
}

app.Run();

public partial class Program { }
