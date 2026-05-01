using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.API.Models;

public class StockEntry
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal CostPricePerUnit { get; set; }

    [MaxLength(200)]
    public string Supplier { get; set; } = string.Empty;

    [MaxLength(50)]
    public string BatchNumber { get; set; } = string.Empty;

    public DateTime? ExpiryDate { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
