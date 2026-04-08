namespace PharmacyManagement.API.DTOs;

public record ProductDto(
    int Id, string Name, string GenericName, string Category,
    string Manufacturer, decimal UnitPrice, decimal CostPrice,
    int QuantityInStock, int ReorderLevel, DateTime? ExpiryDate,
    string BatchNumber, bool IsActive, bool IsLowStock);

public record CreateProductDto(
    string Name, string GenericName, string Category,
    string Manufacturer, decimal UnitPrice, decimal CostPrice,
    int QuantityInStock, int ReorderLevel, DateTime? ExpiryDate,
    string BatchNumber);

public record UpdateProductDto(
    string Name, string GenericName, string Category,
    string Manufacturer, decimal UnitPrice, decimal CostPrice,
    int ReorderLevel, DateTime? ExpiryDate, string BatchNumber);

public record StockEntryDto(
    int Id, int ProductId, string ProductName, int Quantity,
    decimal CostPricePerUnit, string Supplier, string BatchNumber,
    DateTime? ExpiryDate, DateTime EntryDate, string Notes);

public record CreateStockEntryDto(
    int ProductId, int Quantity, decimal CostPricePerUnit,
    string Supplier, string BatchNumber, DateTime? ExpiryDate, string Notes);

public record SaleDto(
    int Id, string InvoiceNumber, string CustomerName,
    DateTime SaleDate, decimal TotalAmount, decimal TotalCost,
    decimal Profit, List<SaleItemDto> Items);

public record SaleItemDto(
    int Id, int ProductId, string ProductName,
    int Quantity, decimal UnitPrice, decimal CostPrice, decimal SubTotal);

public record CreateSaleDto(
    string CustomerName, List<CreateSaleItemDto> Items);

public record CreateSaleItemDto(
    int ProductId, int Quantity);

public record DashboardDto(
    decimal TotalRevenue, decimal TotalCost, decimal NetProfit,
    decimal ProfitMargin, int TotalProducts, int LowStockCount,
    int TotalSales, List<MonthlySummaryDto> MonthlySummaries,
    List<CategorySummaryDto> CategorySummaries,
    List<ProductDto> LowStockProducts);

public record MonthlySummaryDto(
    string Month, decimal Revenue, decimal Cost, decimal Profit);

public record CategorySummaryDto(
    string Category, decimal Revenue, int UnitsSold);

public record LowStockAlertDto(
    int ProductId, string ProductName, int QuantityInStock,
    int ReorderLevel, string Category);
