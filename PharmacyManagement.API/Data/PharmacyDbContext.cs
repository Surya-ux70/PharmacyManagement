using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.API.Models;

namespace PharmacyManagement.API.Data;

public class PharmacyDbContext : IdentityDbContext<ApplicationUser>
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockEntry> StockEntries => Set<StockEntry>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Name);
            e.HasIndex(p => p.Category);
            e.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<StockEntry>(e =>
        {
            e.HasOne(s => s.Product)
             .WithMany(p => p.StockEntries)
             .HasForeignKey(s => s.ProductId);
            e.Property(s => s.CostPricePerUnit).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Sale>(e =>
        {
            e.HasIndex(s => s.InvoiceNumber).IsUnique();
            e.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.TotalCost).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<SaleItem>(e =>
        {
            e.HasOne(si => si.Sale)
             .WithMany(s => s.Items)
             .HasForeignKey(si => si.SaleId);
            e.HasOne(si => si.Product)
             .WithMany(p => p.SaleItems)
             .HasForeignKey(si => si.ProductId);
            e.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(si => si.CostPrice).HasColumnType("decimal(18,2)");
            e.Property(si => si.SubTotal).HasColumnType("decimal(18,2)");
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Paracetamol 500mg", GenericName = "Acetaminophen", Category = "Analgesics", Manufacturer = "PharmaCorp", UnitPrice = 5.99m, CostPrice = 3.20m, QuantityInStock = 500, ReorderLevel = 50, BatchNumber = "PCM-2025-001", ExpiryDate = new DateTime(2027, 6, 15) },
            new Product { Id = 2, Name = "Amoxicillin 250mg", GenericName = "Amoxicillin", Category = "Antibiotics", Manufacturer = "MedLife Labs", UnitPrice = 12.50m, CostPrice = 7.80m, QuantityInStock = 200, ReorderLevel = 30, BatchNumber = "AMX-2025-042", ExpiryDate = new DateTime(2027, 3, 20) },
            new Product { Id = 3, Name = "Omeprazole 20mg", GenericName = "Omeprazole", Category = "Gastrointestinal", Manufacturer = "HealthGen", UnitPrice = 8.75m, CostPrice = 4.50m, QuantityInStock = 8, ReorderLevel = 25, BatchNumber = "OMP-2025-018", ExpiryDate = new DateTime(2027, 9, 10) },
            new Product { Id = 4, Name = "Metformin 500mg", GenericName = "Metformin HCl", Category = "Antidiabetics", Manufacturer = "DiaCare Pharma", UnitPrice = 6.25m, CostPrice = 3.75m, QuantityInStock = 350, ReorderLevel = 40, BatchNumber = "MTF-2025-005", ExpiryDate = new DateTime(2027, 12, 1) },
            new Product { Id = 5, Name = "Atorvastatin 10mg", GenericName = "Atorvastatin Calcium", Category = "Cardiovascular", Manufacturer = "HeartWell Inc", UnitPrice = 15.00m, CostPrice = 9.20m, QuantityInStock = 5, ReorderLevel = 20, BatchNumber = "ATV-2025-033", ExpiryDate = new DateTime(2027, 7, 25) },
            new Product { Id = 6, Name = "Cetirizine 10mg", GenericName = "Cetirizine HCl", Category = "Antihistamines", Manufacturer = "AllerFree Labs", UnitPrice = 4.50m, CostPrice = 2.10m, QuantityInStock = 600, ReorderLevel = 60, BatchNumber = "CTZ-2025-011", ExpiryDate = new DateTime(2028, 1, 15) },
            new Product { Id = 7, Name = "Losartan 50mg", GenericName = "Losartan Potassium", Category = "Cardiovascular", Manufacturer = "HeartWell Inc", UnitPrice = 11.25m, CostPrice = 6.50m, QuantityInStock = 180, ReorderLevel = 25, BatchNumber = "LST-2025-027", ExpiryDate = new DateTime(2027, 11, 5) },
            new Product { Id = 8, Name = "Azithromycin 500mg", GenericName = "Azithromycin", Category = "Antibiotics", Manufacturer = "MedLife Labs", UnitPrice = 18.00m, CostPrice = 11.50m, QuantityInStock = 3, ReorderLevel = 15, BatchNumber = "AZM-2025-009", ExpiryDate = new DateTime(2027, 4, 30) },
            new Product { Id = 9, Name = "Ibuprofen 400mg", GenericName = "Ibuprofen", Category = "Analgesics", Manufacturer = "PharmaCorp", UnitPrice = 6.00m, CostPrice = 3.00m, QuantityInStock = 450, ReorderLevel = 50, BatchNumber = "IBP-2025-015", ExpiryDate = new DateTime(2027, 8, 20) },
            new Product { Id = 10, Name = "Vitamin D3 1000IU", GenericName = "Cholecalciferol", Category = "Vitamins", Manufacturer = "NutriHealth", UnitPrice = 9.99m, CostPrice = 5.00m, QuantityInStock = 320, ReorderLevel = 35, BatchNumber = "VTD-2025-022", ExpiryDate = new DateTime(2028, 5, 10) }
        );

        modelBuilder.Entity<Sale>().HasData(
            new Sale { Id = 1, InvoiceNumber = "INV-2026-0001", CustomerName = "John Smith", SaleDate = new DateTime(2026, 3, 1), TotalAmount = 35.97m, TotalCost = 19.20m },
            new Sale { Id = 2, InvoiceNumber = "INV-2026-0002", CustomerName = "Sarah Johnson", SaleDate = new DateTime(2026, 3, 5), TotalAmount = 56.25m, TotalCost = 35.10m },
            new Sale { Id = 3, InvoiceNumber = "INV-2026-0003", CustomerName = "Mike Davis", SaleDate = new DateTime(2026, 3, 10), TotalAmount = 27.00m, TotalCost = 13.50m },
            new Sale { Id = 4, InvoiceNumber = "INV-2026-0004", CustomerName = "Emily Clark", SaleDate = new DateTime(2026, 3, 15), TotalAmount = 43.74m, TotalCost = 22.50m },
            new Sale { Id = 5, InvoiceNumber = "INV-2026-0005", CustomerName = "David Wilson", SaleDate = new DateTime(2026, 3, 20), TotalAmount = 90.00m, TotalCost = 55.20m },
            new Sale { Id = 6, InvoiceNumber = "INV-2026-0006", CustomerName = "Lisa Brown", SaleDate = new DateTime(2026, 3, 25), TotalAmount = 18.00m, TotalCost = 9.60m },
            new Sale { Id = 7, InvoiceNumber = "INV-2026-0007", CustomerName = "James Taylor", SaleDate = new DateTime(2026, 3, 28), TotalAmount = 62.50m, TotalCost = 39.00m }
        );

        modelBuilder.Entity<SaleItem>().HasData(
            new SaleItem { Id = 1, SaleId = 1, ProductId = 1, Quantity = 6, UnitPrice = 5.99m, CostPrice = 3.20m, SubTotal = 35.94m },
            new SaleItem { Id = 2, SaleId = 2, ProductId = 2, Quantity = 3, UnitPrice = 12.50m, CostPrice = 7.80m, SubTotal = 37.50m },
            new SaleItem { Id = 3, SaleId = 2, ProductId = 3, Quantity = 2, UnitPrice = 8.75m, CostPrice = 4.50m, SubTotal = 17.50m },
            new SaleItem { Id = 4, SaleId = 3, ProductId = 6, Quantity = 6, UnitPrice = 4.50m, CostPrice = 2.10m, SubTotal = 27.00m },
            new SaleItem { Id = 5, SaleId = 4, ProductId = 9, Quantity = 5, UnitPrice = 6.00m, CostPrice = 3.00m, SubTotal = 30.00m },
            new SaleItem { Id = 6, SaleId = 4, ProductId = 1, Quantity = 2, UnitPrice = 5.99m, CostPrice = 3.20m, SubTotal = 11.98m },
            new SaleItem { Id = 7, SaleId = 5, ProductId = 5, Quantity = 4, UnitPrice = 15.00m, CostPrice = 9.20m, SubTotal = 60.00m },
            new SaleItem { Id = 8, SaleId = 5, ProductId = 7, Quantity = 2, UnitPrice = 11.25m, CostPrice = 6.50m, SubTotal = 22.50m },
            new SaleItem { Id = 9, SaleId = 6, ProductId = 1, Quantity = 3, UnitPrice = 5.99m, CostPrice = 3.20m, SubTotal = 17.97m },
            new SaleItem { Id = 10, SaleId = 7, ProductId = 2, Quantity = 5, UnitPrice = 12.50m, CostPrice = 7.80m, SubTotal = 62.50m }
        );

        modelBuilder.Entity<StockEntry>().HasData(
            new StockEntry { Id = 1, ProductId = 1, Quantity = 200, CostPricePerUnit = 3.20m, Supplier = "PharmaCorp Direct", BatchNumber = "PCM-2025-001", EntryDate = new DateTime(2026, 1, 10), ExpiryDate = new DateTime(2027, 6, 15) },
            new StockEntry { Id = 2, ProductId = 2, Quantity = 100, CostPricePerUnit = 7.80m, Supplier = "MedLife Wholesale", BatchNumber = "AMX-2025-042", EntryDate = new DateTime(2026, 1, 15), ExpiryDate = new DateTime(2027, 3, 20) },
            new StockEntry { Id = 3, ProductId = 3, Quantity = 80, CostPricePerUnit = 4.50m, Supplier = "HealthGen Supply", BatchNumber = "OMP-2025-018", EntryDate = new DateTime(2026, 2, 1), ExpiryDate = new DateTime(2027, 9, 10) },
            new StockEntry { Id = 4, ProductId = 4, Quantity = 150, CostPricePerUnit = 3.75m, Supplier = "DiaCare Wholesale", BatchNumber = "MTF-2025-005", EntryDate = new DateTime(2026, 2, 10), ExpiryDate = new DateTime(2027, 12, 1) },
            new StockEntry { Id = 5, ProductId = 5, Quantity = 50, CostPricePerUnit = 9.20m, Supplier = "HeartWell Direct", BatchNumber = "ATV-2025-033", EntryDate = new DateTime(2026, 2, 20), ExpiryDate = new DateTime(2027, 7, 25) },
            new StockEntry { Id = 6, ProductId = 6, Quantity = 300, CostPricePerUnit = 2.10m, Supplier = "AllerFree Wholesale", BatchNumber = "CTZ-2025-011", EntryDate = new DateTime(2026, 3, 1), ExpiryDate = new DateTime(2028, 1, 15) }
        );
    }
}
