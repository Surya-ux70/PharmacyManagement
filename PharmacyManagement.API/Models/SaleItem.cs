namespace PharmacyManagement.API.Models;

public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SubTotal { get; set; }

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
