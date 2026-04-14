using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.API.Data;
using PharmacyManagement.API.DTOs;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly PharmacyDbContext _db;
    public DashboardController(PharmacyDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var sales = await _db.Sales.ToListAsync();
        var saleItems = await _db.SaleItems.Include(si => si.Product).ToListAsync();
        var products = await _db.Products.Where(p => p.IsActive).ToListAsync();

        var totalRevenue = sales.Sum(s => s.TotalAmount);
        var totalCost = sales.Sum(s => s.TotalCost);
        var netProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? Math.Round(netProfit / totalRevenue * 100, 2) : 0;
        var totalProducts = products.Count;
        var lowStockCount = products.Count(p => p.QuantityInStock <= p.ReorderLevel);
        var totalSalesCount = sales.Count;

        var monthlySummaries = sales
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .Select(g => new MonthlySummaryDto(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Sum(s => s.TotalAmount),
                g.Sum(s => s.TotalCost),
                g.Sum(s => s.TotalAmount) - g.Sum(s => s.TotalCost)))
            .OrderBy(m => m.Month)
            .ToList();

        var categorySummaries = saleItems
            .GroupBy(si => si.Product.Category)
            .Select(g => new CategorySummaryDto(
                g.Key,
                g.Sum(si => si.SubTotal),
                g.Sum(si => si.Quantity)))
            .OrderByDescending(c => c.Revenue)
            .ToList();

        var lowStockProducts = products
            .Where(p => p.QuantityInStock <= p.ReorderLevel)
            .OrderBy(p => p.QuantityInStock)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.GenericName, p.Category, p.Manufacturer,
                p.UnitPrice, p.CostPrice, p.QuantityInStock, p.ReorderLevel,
                p.ExpiryDate, p.BatchNumber, p.IsActive, true))
            .ToList();

        return new DashboardDto(
            totalRevenue, totalCost, netProfit, profitMargin,
            totalProducts, lowStockCount, totalSalesCount,
            monthlySummaries, categorySummaries, lowStockProducts);
    }
}
