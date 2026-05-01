using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PharmacyManagement.API.Migrations
{
    public partial class AddMultiTenancy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Delete all seed data first
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 7);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 8);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 9);
            migrationBuilder.DeleteData(table: "SaleItems", keyColumn: "Id", keyValue: 10);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "StockEntries", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 7);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 8);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 9);
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: 10);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Sales", keyColumn: "Id", keyValue: 7);

            // 2. Create Tenants table before adding FK columns
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            // 3. Insert a fallback tenant for any leftover data
            migrationBuilder.Sql(@"
                INSERT INTO ""Tenants"" (""Name"", ""Slug"", ""IsActive"", ""CreatedAt"")
                SELECT 'Legacy Pharmacy', 'legacy-pharmacy', true, NOW()
                WHERE EXISTS (
                    SELECT 1 FROM ""Products""
                    UNION SELECT 1 FROM ""Sales""
                    UNION SELECT 1 FROM ""StockEntries""
                );
            ");

            // 4. Add TenantId columns (nullable first to avoid default constraint issues)
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Sales",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SaleItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StockEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            // 5. Assign existing rows to the legacy tenant
            migrationBuilder.Sql(@"
                UPDATE ""Products"" SET ""TenantId"" = (SELECT ""Id"" FROM ""Tenants"" WHERE ""Slug"" = 'legacy-pharmacy') WHERE ""TenantId"" IS NULL;
                UPDATE ""Sales"" SET ""TenantId"" = (SELECT ""Id"" FROM ""Tenants"" WHERE ""Slug"" = 'legacy-pharmacy') WHERE ""TenantId"" IS NULL;
                UPDATE ""SaleItems"" SET ""TenantId"" = (SELECT ""Id"" FROM ""Tenants"" WHERE ""Slug"" = 'legacy-pharmacy') WHERE ""TenantId"" IS NULL;
                UPDATE ""StockEntries"" SET ""TenantId"" = (SELECT ""Id"" FROM ""Tenants"" WHERE ""Slug"" = 'legacy-pharmacy') WHERE ""TenantId"" IS NULL;
            ");

            // 6. Make TenantId non-nullable on domain tables
            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "SaleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "StockEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // 7. Add indexes
            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_TenantId",
                table: "StockEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId",
                table: "Sales",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_TenantId",
                table: "SaleItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            // 8. Add foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Tenants_TenantId",
                table: "SaleItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Tenants_TenantId",
                table: "Sales",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockEntries_Tenants_TenantId",
                table: "StockEntries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_AspNetUsers_Tenants_TenantId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_Products_Tenants_TenantId", table: "Products");
            migrationBuilder.DropForeignKey(name: "FK_SaleItems_Tenants_TenantId", table: "SaleItems");
            migrationBuilder.DropForeignKey(name: "FK_Sales_Tenants_TenantId", table: "Sales");
            migrationBuilder.DropForeignKey(name: "FK_StockEntries_Tenants_TenantId", table: "StockEntries");

            migrationBuilder.DropTable(name: "Tenants");

            migrationBuilder.DropIndex(name: "IX_StockEntries_TenantId", table: "StockEntries");
            migrationBuilder.DropIndex(name: "IX_Sales_TenantId", table: "Sales");
            migrationBuilder.DropIndex(name: "IX_SaleItems_TenantId", table: "SaleItems");
            migrationBuilder.DropIndex(name: "IX_Products_TenantId", table: "Products");
            migrationBuilder.DropIndex(name: "IX_AspNetUsers_TenantId", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "TenantId", table: "StockEntries");
            migrationBuilder.DropColumn(name: "TenantId", table: "Sales");
            migrationBuilder.DropColumn(name: "TenantId", table: "SaleItems");
            migrationBuilder.DropColumn(name: "TenantId", table: "Products");
            migrationBuilder.DropColumn(name: "TenantId", table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BatchNumber", "Category", "CostPrice", "CreatedAt", "ExpiryDate", "GenericName", "IsActive", "Manufacturer", "Name", "QuantityInStock", "ReorderLevel", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "PCM-2025-001", "Analgesics", 3.20m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6586), new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Acetaminophen", true, "PharmaCorp", "Paracetamol 500mg", 500, 50, 5.99m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6590) },
                    { 2, "AMX-2025-042", "Antibiotics", 7.80m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6609), new DateTime(2027, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amoxicillin", true, "MedLife Labs", "Amoxicillin 250mg", 200, 30, 12.50m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6609) },
                    { 3, "OMP-2025-018", "Gastrointestinal", 4.50m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6615), new DateTime(2027, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Omeprazole", true, "HealthGen", "Omeprazole 20mg", 8, 25, 8.75m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6615) },
                    { 4, "MTF-2025-005", "Antidiabetics", 3.75m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6618), new DateTime(2027, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Metformin HCl", true, "DiaCare Pharma", "Metformin 500mg", 350, 40, 6.25m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6618) },
                    { 5, "ATV-2025-033", "Cardiovascular", 9.20m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6621), new DateTime(2027, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Atorvastatin Calcium", true, "HeartWell Inc", "Atorvastatin 10mg", 5, 20, 15.00m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6622) },
                    { 6, "CTZ-2025-011", "Antihistamines", 2.10m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6625), new DateTime(2028, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cetirizine HCl", true, "AllerFree Labs", "Cetirizine 10mg", 600, 60, 4.50m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6625) },
                    { 7, "LST-2025-027", "Cardiovascular", 6.50m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6629), new DateTime(2027, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Losartan Potassium", true, "HeartWell Inc", "Losartan 50mg", 180, 25, 11.25m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6629) },
                    { 8, "AZM-2025-009", "Antibiotics", 11.50m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6633), new DateTime(2027, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Azithromycin", true, "MedLife Labs", "Azithromycin 500mg", 3, 15, 18.00m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6634) },
                    { 9, "IBP-2025-015", "Analgesics", 3.00m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6637), new DateTime(2027, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ibuprofen", true, "PharmaCorp", "Ibuprofen 400mg", 450, 50, 6.00m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6637) },
                    { 10, "VTD-2025-022", "Vitamins", 5.00m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6690), new DateTime(2028, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cholecalciferol", true, "NutriHealth", "Vitamin D3 1000IU", 320, 35, 9.99m, new DateTime(2026, 4, 14, 6, 48, 0, 835, DateTimeKind.Utc).AddTicks(6690) }
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
        }
    }
}
