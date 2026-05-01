using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.API.Models;
using PharmacyManagement.API.Services;

namespace PharmacyManagement.API.Data;

public class PharmacyDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly int? _tenantId;

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantId = tenantService.TenantId;
    }

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockEntry> StockEntries => Set<StockEntry>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasIndex(t => t.Slug).IsUnique();
        });

        modelBuilder.Entity<ApplicationUser>(e =>
        {
            e.HasOne(u => u.Tenant)
             .WithMany()
             .HasForeignKey(u => u.TenantId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasQueryFilter(p => _tenantId == null || p.TenantId == _tenantId);
            e.HasIndex(p => p.Name);
            e.HasIndex(p => p.Category);
            e.HasIndex(p => p.TenantId);
            e.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Tenant).WithMany().HasForeignKey(p => p.TenantId);
        });

        modelBuilder.Entity<StockEntry>(e =>
        {
            e.HasQueryFilter(s => _tenantId == null || s.TenantId == _tenantId);
            e.HasOne(s => s.Product)
             .WithMany(p => p.StockEntries)
             .HasForeignKey(s => s.ProductId);
            e.HasIndex(s => s.TenantId);
            e.Property(s => s.CostPricePerUnit).HasColumnType("decimal(18,2)");
            e.HasOne(s => s.Tenant).WithMany().HasForeignKey(s => s.TenantId);
        });

        modelBuilder.Entity<Sale>(e =>
        {
            e.HasQueryFilter(s => _tenantId == null || s.TenantId == _tenantId);
            e.HasIndex(s => s.InvoiceNumber).IsUnique();
            e.HasIndex(s => s.TenantId);
            e.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.TotalCost).HasColumnType("decimal(18,2)");
            e.HasOne(s => s.Tenant).WithMany().HasForeignKey(s => s.TenantId);
        });

        modelBuilder.Entity<SaleItem>(e =>
        {
            e.HasQueryFilter(si => _tenantId == null || si.TenantId == _tenantId);
            e.HasOne(si => si.Sale)
             .WithMany(s => s.Items)
             .HasForeignKey(si => si.SaleId);
            e.HasOne(si => si.Product)
             .WithMany(p => p.SaleItems)
             .HasForeignKey(si => si.ProductId);
            e.HasIndex(si => si.TenantId);
            e.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(si => si.CostPrice).HasColumnType("decimal(18,2)");
            e.Property(si => si.SubTotal).HasColumnType("decimal(18,2)");
            e.HasOne(si => si.Tenant).WithMany().HasForeignKey(si => si.TenantId);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetTenantId();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetTenantId();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetTenantId()
    {
        if (_tenantId == null) return;

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            if (entry.Entity is Product p && p.TenantId == 0) p.TenantId = _tenantId.Value;
            else if (entry.Entity is Sale s && s.TenantId == 0) s.TenantId = _tenantId.Value;
            else if (entry.Entity is SaleItem si && si.TenantId == 0) si.TenantId = _tenantId.Value;
            else if (entry.Entity is StockEntry se && se.TenantId == 0) se.TenantId = _tenantId.Value;
        }
    }
}
