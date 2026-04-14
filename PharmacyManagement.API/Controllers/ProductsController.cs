using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.API.Data;
using PharmacyManagement.API.DTOs;
using PharmacyManagement.API.Models;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly PharmacyDbContext _db;
    public ProductsController(PharmacyDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] string? search, [FromQuery] string? category)
    {
        var query = _db.Products.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.GenericName.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        var products = await query.OrderBy(p => p.Name).ToListAsync();
        return products.Select(ToDto).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        return ToDto(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            GenericName = dto.GenericName,
            Category = dto.Category,
            Manufacturer = dto.Manufacturer,
            UnitPrice = dto.UnitPrice,
            CostPrice = dto.CostPrice,
            QuantityInStock = dto.QuantityInStock,
            ReorderLevel = dto.ReorderLevel,
            ExpiryDate = dto.ExpiryDate,
            BatchNumber = dto.BatchNumber
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = product.Id }, ToDto(product));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = dto.Name;
        product.GenericName = dto.GenericName;
        product.Category = dto.Category;
        product.Manufacturer = dto.Manufacturer;
        product.UnitPrice = dto.UnitPrice;
        product.CostPrice = dto.CostPrice;
        product.ReorderLevel = dto.ReorderLevel;
        product.ExpiryDate = dto.ExpiryDate;
        product.BatchNumber = dto.BatchNumber;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        return await _db.Products
            .Where(p => p.IsActive)
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<LowStockAlertDto>>> GetLowStock()
    {
        return await _db.Products
            .Where(p => p.IsActive && p.QuantityInStock <= p.ReorderLevel)
            .Select(p => new LowStockAlertDto(p.Id, p.Name, p.QuantityInStock, p.ReorderLevel, p.Category))
            .ToListAsync();
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.GenericName, p.Category, p.Manufacturer,
        p.UnitPrice, p.CostPrice, p.QuantityInStock, p.ReorderLevel,
        p.ExpiryDate, p.BatchNumber, p.IsActive,
        p.QuantityInStock <= p.ReorderLevel);
}
