using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PharmacyManagement.API.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GenericName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    ReorderLevel = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BatchNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SaleDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CostPricePerUnit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Supplier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockEntries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BatchNumber", "Category", "CostPrice", "CreatedAt", "ExpiryDate", "GenericName", "IsActive", "Manufacturer", "Name", "QuantityInStock", "ReorderLevel", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "PCM-2025-001", "Analgesics", 3.20m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7250), new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Acetaminophen", true, "PharmaCorp", "Paracetamol 500mg", 500, 50, 5.99m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7252) },
                    { 2, "AMX-2025-042", "Antibiotics", 7.80m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7324), new DateTime(2027, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amoxicillin", true, "MedLife Labs", "Amoxicillin 250mg", 200, 30, 12.50m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7324) },
                    { 3, "OMP-2025-018", "Gastrointestinal", 4.50m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7327), new DateTime(2027, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Omeprazole", true, "HealthGen", "Omeprazole 20mg", 8, 25, 8.75m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7327) },
                    { 4, "MTF-2025-005", "Antidiabetics", 3.75m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7329), new DateTime(2027, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Metformin HCl", true, "DiaCare Pharma", "Metformin 500mg", 350, 40, 6.25m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7330) },
                    { 5, "ATV-2025-033", "Cardiovascular", 9.20m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7331), new DateTime(2027, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Atorvastatin Calcium", true, "HeartWell Inc", "Atorvastatin 10mg", 5, 20, 15.00m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7332) },
                    { 6, "CTZ-2025-011", "Antihistamines", 2.10m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7333), new DateTime(2028, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cetirizine HCl", true, "AllerFree Labs", "Cetirizine 10mg", 600, 60, 4.50m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7334) },
                    { 7, "LST-2025-027", "Cardiovascular", 6.50m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7336), new DateTime(2027, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Losartan Potassium", true, "HeartWell Inc", "Losartan 50mg", 180, 25, 11.25m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7336) },
                    { 8, "AZM-2025-009", "Antibiotics", 11.50m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7338), new DateTime(2027, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Azithromycin", true, "MedLife Labs", "Azithromycin 500mg", 3, 15, 18.00m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7338) },
                    { 9, "IBP-2025-015", "Analgesics", 3.00m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7345), new DateTime(2027, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ibuprofen", true, "PharmaCorp", "Ibuprofen 400mg", 450, 50, 6.00m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7345) },
                    { 10, "VTD-2025-022", "Vitamins", 5.00m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7347), new DateTime(2028, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cholecalciferol", true, "NutriHealth", "Vitamin D3 1000IU", 320, 35, 9.99m, new DateTime(2026, 4, 14, 6, 47, 39, 734, DateTimeKind.Utc).AddTicks(7347) }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CustomerName", "InvoiceNumber", "SaleDate", "TotalAmount", "TotalCost" },
                values: new object[,]
                {
                    { 1, "John Smith", "INV-2026-0001", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 35.97m, 19.20m },
                    { 2, "Sarah Johnson", "INV-2026-0002", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 56.25m, 35.10m },
                    { 3, "Mike Davis", "INV-2026-0003", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 27.00m, 13.50m },
                    { 4, "Emily Clark", "INV-2026-0004", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 43.74m, 22.50m },
                    { 5, "David Wilson", "INV-2026-0005", new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 90.00m, 55.20m },
                    { 6, "Lisa Brown", "INV-2026-0006", new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 18.00m, 9.60m },
                    { 7, "James Taylor", "INV-2026-0007", new DateTime(2026, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 62.50m, 39.00m }
                });

            migrationBuilder.InsertData(
                table: "SaleItems",
                columns: new[] { "Id", "CostPrice", "ProductId", "Quantity", "SaleId", "SubTotal", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 3.20m, 1, 6, 1, 35.94m, 5.99m },
                    { 2, 7.80m, 2, 3, 2, 37.50m, 12.50m },
                    { 3, 4.50m, 3, 2, 2, 17.50m, 8.75m },
                    { 4, 2.10m, 6, 6, 3, 27.00m, 4.50m },
                    { 5, 3.00m, 9, 5, 4, 30.00m, 6.00m },
                    { 6, 3.20m, 1, 2, 4, 11.98m, 5.99m },
                    { 7, 9.20m, 5, 4, 5, 60.00m, 15.00m },
                    { 8, 6.50m, 7, 2, 5, 22.50m, 11.25m },
                    { 9, 3.20m, 1, 3, 6, 17.97m, 5.99m },
                    { 10, 7.80m, 2, 5, 7, 62.50m, 12.50m }
                });

            migrationBuilder.InsertData(
                table: "StockEntries",
                columns: new[] { "Id", "BatchNumber", "CostPricePerUnit", "EntryDate", "ExpiryDate", "Notes", "ProductId", "Quantity", "Supplier" },
                values: new object[,]
                {
                    { 1, "PCM-2025-001", 3.20m, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 1, 200, "PharmaCorp Direct" },
                    { 2, "AMX-2025-042", 7.80m, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 2, 100, "MedLife Wholesale" },
                    { 3, "OMP-2025-018", 4.50m, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 3, 80, "HealthGen Supply" },
                    { 4, "MTF-2025-005", 3.75m, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 4, 150, "DiaCare Wholesale" },
                    { 5, "ATV-2025-033", 9.20m, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 5, 50, "HeartWell Direct" },
                    { 6, "CTZ-2025-011", 2.10m, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2028, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 6, 300, "AllerFree Wholesale" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_ProductId",
                table: "StockEntries",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
