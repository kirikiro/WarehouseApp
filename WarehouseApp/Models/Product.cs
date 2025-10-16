using System;
using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models;

public class Product  // class mutable
{
    public int Id { get; set; }  // set для edit
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;  // set для edit
}