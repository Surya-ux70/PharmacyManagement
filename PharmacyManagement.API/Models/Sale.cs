using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.API.Models;

public class Sale
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public decimal TotalCost { get; set; }

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
