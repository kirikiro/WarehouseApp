using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseApp.Data;
using WarehouseApp.Models;

namespace WarehouseApp.Pages;

public class IndexModel : PageModel
{
    private readonly IProductRepository _repository;
    private readonly ILogger<IndexModel> _logger;  // логгер

    public IndexModel(IProductRepository repository, ILogger<IndexModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string? NameFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MinQuantity { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();

    [BindProperty]
    public Product NewProduct { get; set; } = new();

    public async Task OnGetAsync()
    {
        Products = await _repository.GetProductsAsync(NameFilter, MinPrice, MaxPrice, MinQuantity, SortBy);
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState invalid: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            await OnGetAsync();
            return Page();  // Вернёт страницу с ошибками
        }

        try
        {
            if (NewProduct.Id > 0)
            {
                var existing = await _repository.GetProductAsync(NewProduct.Id);
                if (existing != null)
                {
                    existing.Name = NewProduct.Name;
                    existing.Description = NewProduct.Description;
                    existing.Price = NewProduct.Price;
                    existing.Quantity = NewProduct.Quantity;
                    await _repository.SaveChangesAsync();
                    _logger.LogInformation("Updated product Id={Id}", existing.Id);
                }
                else
                {
                    _logger.LogWarning("Product Id={Id} not found for update", NewProduct.Id);
                }
            }
            else
            {
                NewProduct.DateAdded = DateTime.UtcNow;  // Явная установка даты
                await _repository.AddProductAsync(NewProduct);
                _logger.LogInformation("Added new product Id={Id}", NewProduct.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving product");
            ModelState.AddModelError("", "Ошибка сохранения: " + ex.Message);
            await OnGetAsync();
            return Page();
        }

        return RedirectToPage();  // Релоад страницы
    }
}