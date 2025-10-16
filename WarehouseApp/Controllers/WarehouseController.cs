using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Data;
using WarehouseApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarehouseApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IProductRepository _repository;

    public WarehouseController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] string? nameFilter = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? minQuantity = null,
        [FromQuery] string? sortBy = null)
    {
        var products = await _repository.GetProductsAsync(nameFilter, minPrice, maxPrice, minQuantity, sortBy);
        return Ok(products);
    }

[HttpPost]
    public async Task<ActionResult<Product>> AddProduct(Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        product.DateAdded = DateTime.UtcNow;  // прямое присваивание вместо with
        var added = await _repository.AddProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = added.Id }, added);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repository.GetProductAsync(id);
        return product != null ? Ok(product) : NotFound();
    }
}
