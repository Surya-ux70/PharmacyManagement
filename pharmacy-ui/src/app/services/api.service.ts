import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Product, CreateProduct, UpdateProduct,
  StockEntry, CreateStockEntry,
  Sale, CreateSale,
  Dashboard, LowStockAlert, Tenant
} from '../models/pharmacy.models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Products
  getProducts(search?: string, category?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (category) params = params.set('category', category);
    return this.http.get<Product[]>(`${this.baseUrl}/products`, { params });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/products/${id}`);
  }

  createProduct(product: CreateProduct): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}/products`, product);
  }

  updateProduct(id: number, product: UpdateProduct): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/products/${id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/products/${id}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/products/categories`);
  }

  getLowStockAlerts(): Observable<LowStockAlert[]> {
    return this.http.get<LowStockAlert[]>(`${this.baseUrl}/products/low-stock`);
  }

  // Stock
  getStockEntries(): Observable<StockEntry[]> {
    return this.http.get<StockEntry[]>(`${this.baseUrl}/stock`);
  }

  createStockEntry(entry: CreateStockEntry): Observable<StockEntry> {
    return this.http.post<StockEntry>(`${this.baseUrl}/stock`, entry);
  }

  // Sales
  getSales(): Observable<Sale[]> {
    return this.http.get<Sale[]>(`${this.baseUrl}/sales`);
  }

  getSale(id: number): Observable<Sale> {
    return this.http.get<Sale>(`${this.baseUrl}/sales/${id}`);
  }

  createSale(sale: CreateSale): Observable<Sale> {
    return this.http.post<Sale>(`${this.baseUrl}/sales`, sale);
  }

  // Dashboard
  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.baseUrl}/dashboard`);
  }

  // Tenants (SuperAdmin)
  getTenants(): Observable<Tenant[]> {
    return this.http.get<Tenant[]>(`${this.baseUrl}/tenants`);
  }

  createTenant(data: { name: string; adminFullName: string; adminEmail: string; adminPassword: string }): Observable<Tenant> {
    return this.http.post<Tenant>(`${this.baseUrl}/tenants`, data);
  }

  updateTenant(id: number, data: { name: string; isActive: boolean }): Observable<Tenant> {
    return this.http.put<Tenant>(`${this.baseUrl}/tenants/${id}`, data);
  }
}
