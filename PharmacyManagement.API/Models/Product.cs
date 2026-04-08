using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.API.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string GenericName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Manufacturer { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public decimal CostPrice { get; set; }

    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; } = 10;

    public DateTime? ExpiryDate { get; set; }

    [MaxLength(50)]
    public string BatchNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
