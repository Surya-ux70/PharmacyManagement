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
public class StockController : ControllerBase
{
    private readonly PharmacyDbContext _db;
    public StockController(PharmacyDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<StockEntryDto>>> GetAll()
    {
        return await _db.StockEntries
            .Include(s => s.Product)
            .OrderByDescending(s => s.EntryDate)
            .Select(s => new StockEntryDto(
                s.Id, s.ProductId, s.Product.Name, s.Quantity,
                s.CostPricePerUnit, s.Supplier, s.BatchNumber,
                s.ExpiryDate, s.EntryDate, s.Notes))
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<StockEntryDto>> Create(CreateStockEntryDto dto)
    {
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product == null) return BadRequest("Product not found");

        var entry = new StockEntry
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            CostPricePerUnit = dto.CostPricePerUnit,
            Supplier = dto.Supplier,
            BatchNumber = dto.BatchNumber,
            ExpiryDate = dto.ExpiryDate,
            Notes = dto.Notes
        };

        product.QuantityInStock += dto.Quantity;
        product.CostPrice = dto.CostPricePerUnit;
        if (dto.ExpiryDate.HasValue) product.ExpiryDate = dto.ExpiryDate;
        if (!string.IsNullOrWhiteSpace(dto.BatchNumber)) product.BatchNumber = dto.BatchNumber;
        product.UpdatedAt = DateTime.UtcNow;

        _db.StockEntries.Add(entry);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), null,
            new StockEntryDto(entry.Id, entry.ProductId, product.Name,
                entry.Quantity, entry.CostPricePerUnit, entry.Supplier,
                entry.BatchNumber, entry.ExpiryDate, entry.EntryDate, entry.Notes));
    }
}
