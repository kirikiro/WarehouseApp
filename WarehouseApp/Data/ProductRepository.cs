using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;                 
using CsvHelper;                
using System.Globalization;     

namespace WarehouseApp.Data;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(string? nameFilter = null, decimal? minPrice = null, decimal? maxPrice = null, int? minQuantity = null, string? sortBy = null);
    Task<Product?> GetProductAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task SaveChangesAsync();
    Task ExportToCsvAsync(string csvPath);  
}

public class ProductRepository : IProductRepository
{
    private readonly WarehouseContext _context;

    public ProductRepository(WarehouseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(string? nameFilter = null, decimal? minPrice = null, decimal? maxPrice = null, int? minQuantity = null, string? sortBy = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(nameFilter))
            query = query.Where(p => p.Name.Contains(nameFilter));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (minQuantity.HasValue)
            query = query.Where(p => p.Quantity >= minQuantity.Value);

        return sortBy switch
        {
            "nameAsc" => await query.OrderBy(p => p.Name).ToListAsync(),
            "dateDesc" => await query.OrderByDescending(p => p.DateAdded).ToListAsync(),
            _ => await query.OrderBy(p => p.DateAdded).ToListAsync()  // По умолчанию
        };
    }

    public async Task<Product?> GetProductAsync(int id) => await _context.Products.FindAsync(id);

    public async Task<Product> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task ExportToCsvAsync(string csvPath)  
    {
        var products = await _context.Products.ToListAsync();
        using var writer = new StreamWriter(csvPath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(products);
    }
}