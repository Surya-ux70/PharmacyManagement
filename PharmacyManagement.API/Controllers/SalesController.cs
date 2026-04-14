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
public class SalesController : ControllerBase
{
    private readonly PharmacyDbContext _db;
    public SalesController(PharmacyDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<SaleDto>>> GetAll()
    {
        return await _db.Sales
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleDto(
                s.Id, s.InvoiceNumber, s.CustomerName, s.SaleDate,
                s.TotalAmount, s.TotalCost, s.TotalAmount - s.TotalCost,
                s.Items.Select(i => new SaleItemDto(
                    i.Id, i.ProductId, i.Product.Name,
                    i.Quantity, i.UnitPrice, i.CostPrice, i.SubTotal)).ToList()))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> Get(int id)
    {
        var sale = await _db.Sales
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null) return NotFound();

        return new SaleDto(
            sale.Id, sale.InvoiceNumber, sale.CustomerName, sale.SaleDate,
            sale.TotalAmount, sale.TotalCost, sale.TotalAmount - sale.TotalCost,
            sale.Items.Select(i => new SaleItemDto(
                i.Id, i.ProductId, i.Product.Name,
                i.Quantity, i.UnitPrice, i.CostPrice, i.SubTotal)).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> Create(CreateSaleDto dto)
    {
        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{(await _db.Sales.CountAsync() + 1):D4}";
        decimal totalAmount = 0, totalCost = 0;
        var saleItems = new List<SaleItem>();

        foreach (var item in dto.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest($"Product {item.ProductId} not found");
            if (product.QuantityInStock < item.Quantity)
                return BadRequest($"Insufficient stock for {product.Name}. Available: {product.QuantityInStock}");

            var subTotal = product.UnitPrice * item.Quantity;
            var costTotal = product.CostPrice * item.Quantity;
            totalAmount += subTotal;
            totalCost += costTotal;

            saleItems.Add(new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.UnitPrice,
                CostPrice = product.CostPrice,
                SubTotal = subTotal
            });

            product.QuantityInStock -= item.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        var sale = new Sale
        {
            InvoiceNumber = invoiceNumber,
            CustomerName = dto.CustomerName,
            TotalAmount = totalAmount,
            TotalCost = totalCost,
            Items = saleItems
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync();

        await _db.Entry(sale).Collection(s => s.Items).Query().Include(i => i.Product).LoadAsync();

        return CreatedAtAction(nameof(Get), new { id = sale.Id },
            new SaleDto(sale.Id, sale.InvoiceNumber, sale.CustomerName,
                sale.SaleDate, sale.TotalAmount, sale.TotalCost,
                sale.TotalAmount - sale.TotalCost,
                sale.Items.Select(i => new SaleItemDto(
                    i.Id, i.ProductId, i.Product.Name,
                    i.Quantity, i.UnitPrice, i.CostPrice, i.SubTotal)).ToList()));
    }
}
