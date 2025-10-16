using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WarehouseApp.Data;

public class WarehouseContextFactory : IDesignTimeDbContextFactory<WarehouseContext>
{
    public WarehouseContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<WarehouseContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=warehouse.db");

        return new WarehouseContext(optionsBuilder.Options);
    }
}