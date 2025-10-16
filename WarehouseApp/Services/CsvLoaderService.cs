using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WarehouseApp.Data;
using WarehouseApp.Models;
using Microsoft.EntityFrameworkCore; 

namespace WarehouseApp.Services;

public class CsvLoaderService : BackgroundService, ICsvLoaderService
{
    private readonly ILogger<CsvLoaderService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IServiceScopeFactory _scopeFactory;

    public CsvLoaderService(ILogger<CsvLoaderService> logger, IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _env = env;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadCsvAsync();
    }

    public async Task LoadCsvAsync()
    {
        var csvPath = Path.Combine(_env.WebRootPath, "warehouse.csv");
        if (!File.Exists(csvPath))
        {
            _logger.LogWarning("CSV file not found at {Path}", csvPath);
            return;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);

        var products = new List<Product>();
        await foreach (var product in csv.GetRecordsAsync<Product>())
        {
            products.Add(product);
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>();

        // Загружает csv только если БД пуста
        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            _logger.LogInformation("Loaded {Count} products from CSV (initial load)", products.Count);
        }
        else
        {
            _logger.LogInformation("DB already has data, skipping CSV load");
        }
    }
}