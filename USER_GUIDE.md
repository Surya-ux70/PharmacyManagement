# PharmaCare Manager — User Guide

A pharmacy inventory and sales management system. This guide walks you through every feature of the application.

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Dashboard](#dashboard)
3. [Inventory Management](#inventory-management)
4. [Stock Entry](#stock-entry)
5. [Sales](#sales)
6. [Low Stock Alerts](#low-stock-alerts)

---

## Getting Started

Open the application in your browser at **http://localhost:4200**. You will see a sidebar on the left with four sections:

| Section | Purpose |
|---------|---------|
| **Dashboard** | Financial overview with charts |
| **Inventory** | Manage your product catalog |
| **Stock Entry** | Record incoming stock from suppliers |
| **Sales** | Create new sales and view sales history |

Use the **menu icon** (top-left) to collapse or expand the sidebar.

---

## Dashboard

The dashboard gives you a bird's-eye view of your pharmacy's financial health. Everything here is **read-only** — it updates automatically as you add sales and stock.

### KPI Cards

| Card | What it shows |
|------|---------------|
| **Total Revenue** | Sum of all sale amounts (what customers paid) |
| **Cost of Goods Sold** | Sum of what you paid for the products that were sold |
| **Net Profit** | Revenue minus Cost |
| **Profit Margin** | Net Profit as a percentage of Revenue |

### Statistics

- **Products** — Total number of active products in your catalog.
- **Total Sales** — Number of sales transactions completed.
- **Low Stock Items** — Products whose current stock is at or below their reorder level (highlighted in orange when there are any).

### Charts

- **Revenue vs Cost vs Profit** — A monthly bar chart showing how your revenue, costs, and profit trend over time.
- **Revenue by Category** — A doughnut chart showing which product categories bring in the most revenue.

### Low Stock Alerts Table

If any products are running low, they appear in a table at the bottom showing the product name, category, current stock, and the reorder level you set.

---

## Inventory Management

This is where you manage your full product catalog.

### Viewing Products

The inventory page shows a table with all your products, including:

- Product name and generic name
- Category
- Manufacturer
- Cost price (what you pay) and selling price (what the customer pays)
- Current stock quantity (with a warning icon if below reorder level)
- Expiry date

### Searching and Filtering

- **Search box** — Type to search by product name or generic name. Results update as you type.
- **Category dropdown** — Select a specific category or choose "All Categories" to see everything.

### Adding a New Product

1. Click the **"+ Add Product"** button (top-right).
2. Fill in the product details:

| Field | Description |
|-------|-------------|
| Product Name | The brand/trade name (e.g., "Paracetamol 500mg") |
| Generic Name | The drug's generic name (e.g., "Acetaminophen") |
| Category | Product category (e.g., "Analgesics", "Antibiotics") |
| Manufacturer | The manufacturer or supplier company |
| Cost Price | What you pay per unit to purchase this product |
| Selling Price | What you charge the customer per unit |
| Initial Stock | How many units you currently have on hand |
| Reorder Level | The minimum stock at which you want to be alerted |
| Batch Number | The manufacturer's batch identifier |
| Expiry Date | When this batch expires |

3. Click **"Create"** to save the product.

### Editing a Product

1. Click the **pencil icon** on any product row.
2. Update the fields you want to change. Note: you **cannot** change stock quantity here — use the **Stock Entry** page to add stock.
3. Click **"Update"** to save.

### Deleting a Product

1. Click the **trash icon** on the product row.
2. Confirm the deletion in the popup.

> The product is soft-deleted (deactivated), not permanently removed from the database.

---

## Stock Entry

Use this page whenever you receive new stock from a supplier. It has two sections side by side.

### Recording New Stock (left side)

1. **Select the product** from the dropdown. Each option shows the product name and current stock count.
2. Fill in the details:

| Field | Description |
|-------|-------------|
| Product | Which product you're receiving stock for |
| Quantity | Number of units received |
| Cost per Unit | What you paid per unit for this batch |
| Supplier | Who supplied this batch |
| Batch Number | The batch identifier for tracking |
| Expiry Date | When this batch expires |
| Notes | Any additional notes (optional) |

3. Click **"Record Stock Entry"**.

The product's stock count will be updated automatically, and the entry appears in the history table.

### Viewing Stock History (right side)

The "Recent Stock Entries" table shows all previous stock receipts with:

- Date received
- Product name
- Quantity added
- Unit cost
- Supplier
- Total cost (quantity x unit cost)

---

## Sales

Use this page to record customer purchases and review past sales.

### Creating a New Sale (left side)

1. **Customer Name** — Enter the customer's name. If left blank, it defaults to "Walk-in".
2. **Add items to the sale:**
   - Select a **product** from the dropdown (shows available stock).
   - Enter the **quantity** the customer is buying.
   - Click the **"+"** button to add more line items.
   - Click the **"x"** button to remove a line item.
3. The **Estimated Total** updates automatically based on the products and quantities selected.
4. Click **"Complete Sale"** to finalize.

> The system automatically:
> - Generates a unique **invoice number** (e.g., INV-2026-0008).
> - Records the sale date.
> - Deducts the sold quantities from product stock.
> - Calculates revenue, cost, and profit for each line item based on current product prices.

### Viewing Sales History (right side)

Past sales are shown as expandable cards. Each card shows:

- **Invoice number**
- **Customer name**
- **Sale date**
- **Total amount** (highlighted chip)

Click on a sale to expand it and see:

- A table of individual items sold (product, quantity, unit price, subtotal).
- A summary row showing **Revenue**, **Cost**, and **Profit** for that sale.

---

## Low Stock Alerts

The system continuously monitors stock levels. When a product's quantity falls **at or below** its reorder level, it triggers a low stock alert.

### Where alerts appear

- **Toolbar bell icon** — A notification badge appears in the top-right corner showing how many products are low on stock. Clicking it takes you to the Inventory page.
- **Dashboard** — The "Low Stock Items" stat card shows the count, and the alerts table at the bottom lists the specific products.
- **Inventory table** — Low-stock products show a **warning icon** next to their stock count.

### How to resolve

1. Note which products are low from the dashboard or inventory page.
2. Go to **Stock Entry**.
3. Select the product, enter the quantity received from your supplier, and record the entry.
4. The alert will clear once the stock rises above the reorder level.

---

## Quick Reference

| I want to... | Go to |
|--------------|-------|
| See how my business is doing | Dashboard |
| Add a new medicine to the catalog | Inventory → Add Product |
| Update a product's price | Inventory → Edit (pencil icon) |
| Receive stock from a supplier | Stock Entry |
| Sell products to a customer | Sales → New Sale |
| Check which products need reordering | Dashboard (Low Stock table) or Inventory (warning icons) |
| View past sales and profits | Sales → click any sale to expand |
