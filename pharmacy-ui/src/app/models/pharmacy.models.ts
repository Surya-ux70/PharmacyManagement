export interface Product {
  id: number;
  name: string;
  genericName: string;
  category: string;
  manufacturer: string;
  unitPrice: number;
  costPrice: number;
  quantityInStock: number;
  reorderLevel: number;
  expiryDate: string | null;
  batchNumber: string;
  isActive: boolean;
  isLowStock: boolean;
}

export interface CreateProduct {
  name: string;
  genericName: string;
  category: string;
  manufacturer: string;
  unitPrice: number;
  costPrice: number;
  quantityInStock: number;
  reorderLevel: number;
  expiryDate: string | null;
  batchNumber: string;
}

export interface UpdateProduct {
  name: string;
  genericName: string;
  category: string;
  manufacturer: string;
  unitPrice: number;
  costPrice: number;
  reorderLevel: number;
  expiryDate: string | null;
  batchNumber: string;
}

export interface StockEntry {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  costPricePerUnit: number;
  supplier: string;
  batchNumber: string;
  expiryDate: string | null;
  entryDate: string;
  notes: string;
}

export interface CreateStockEntry {
  productId: number;
  quantity: number;
  costPricePerUnit: number;
  supplier: string;
  batchNumber: string;
  expiryDate: string | null;
  notes: string;
}

export interface Sale {
  id: number;
  invoiceNumber: string;
  customerName: string;
  saleDate: string;
  totalAmount: number;
  totalCost: number;
  profit: number;
  items: SaleItem[];
}

export interface SaleItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  costPrice: number;
  subTotal: number;
}

export interface CreateSale {
  customerName: string;
  items: CreateSaleItem[];
}

export interface CreateSaleItem {
  productId: number;
  quantity: number;
}

export interface Dashboard {
  totalRevenue: number;
  totalCost: number;
  netProfit: number;
  profitMargin: number;
  totalProducts: number;
  lowStockCount: number;
  totalSales: number;
  monthlySummaries: MonthlySummary[];
  categorySummaries: CategorySummary[];
  lowStockProducts: Product[];
}

export interface MonthlySummary {
  month: string;
  revenue: number;
  cost: number;
  profit: number;
}

export interface CategorySummary {
  category: string;
  revenue: number;
  unitsSold: number;
}

export interface LowStockAlert {
  productId: number;
  productName: string;
  quantityInStock: number;
  reorderLevel: number;
  category: string;
}
