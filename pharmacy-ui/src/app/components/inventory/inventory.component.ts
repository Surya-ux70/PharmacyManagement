import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Product } from '../../models/pharmacy.models';
import { ProductDialogComponent } from './product-dialog.component';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [
    CommonModule, FormsModule, CurrencyPipe, DatePipe,
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatInputModule, MatFormFieldModule, MatSelectModule,
    MatDialogModule, MatChipsModule, MatTooltipModule, MatSnackBarModule
  ],
  template: `
    <div class="inventory">
      <div class="header">
        <h1 class="page-title">Inventory Management</h1>
        <button mat-raised-button color="primary" (click)="openProductDialog()">
          <mat-icon>add</mat-icon> Add Product
        </button>
      </div>

      <mat-card class="filter-card">
        <mat-card-content>
          <div class="filters">
            <mat-form-field appearance="outline" class="search-field">
              <mat-label>Search products...</mat-label>
              <input matInput [(ngModel)]="search" (input)="loadProducts()">
              <mat-icon matSuffix>search</mat-icon>
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Category</mat-label>
              <mat-select [(ngModel)]="selectedCategory" (selectionChange)="loadProducts()">
                <mat-option value="">All Categories</mat-option>
                @for (cat of categories; track cat) {
                  <mat-option [value]="cat">{{ cat }}</mat-option>
                }
              </mat-select>
            </mat-form-field>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <table mat-table [dataSource]="products" class="product-table">
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef>Product Name</th>
            <td mat-cell *matCellDef="let p">
              <div class="product-name">{{ p.name }}</div>
              <div class="product-generic">{{ p.genericName }}</div>
            </td>
          </ng-container>

          <ng-container matColumnDef="category">
            <th mat-header-cell *matHeaderCellDef>Category</th>
            <td mat-cell *matCellDef="let p"><mat-chip>{{ p.category }}</mat-chip></td>
          </ng-container>

          <ng-container matColumnDef="manufacturer">
            <th mat-header-cell *matHeaderCellDef>Manufacturer</th>
            <td mat-cell *matCellDef="let p">{{ p.manufacturer }}</td>
          </ng-container>

          <ng-container matColumnDef="costPrice">
            <th mat-header-cell *matHeaderCellDef>Cost Price</th>
            <td mat-cell *matCellDef="let p">{{ p.costPrice | currency:'INR' }}</td>
          </ng-container>

          <ng-container matColumnDef="unitPrice">
            <th mat-header-cell *matHeaderCellDef>Sell Price</th>
            <td mat-cell *matCellDef="let p">{{ p.unitPrice | currency:'INR' }}</td>
          </ng-container>

          <ng-container matColumnDef="stock">
            <th mat-header-cell *matHeaderCellDef>Stock</th>
            <td mat-cell *matCellDef="let p" [class.low-stock]="p.isLowStock">
              <span class="stock-badge" [class.danger]="p.isLowStock">
                {{ p.quantityInStock }}
                @if (p.isLowStock) {
                  <mat-icon class="low-icon" matTooltip="Below reorder level ({{ p.reorderLevel }})">warning</mat-icon>
                }
              </span>
            </td>
          </ng-container>

          <ng-container matColumnDef="expiry">
            <th mat-header-cell *matHeaderCellDef>Expiry</th>
            <td mat-cell *matCellDef="let p">{{ p.expiryDate | date:'mediumDate' }}</td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let p">
              <button mat-icon-button color="primary" (click)="openProductDialog(p)" matTooltip="Edit">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="deleteProduct(p)" matTooltip="Delete">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
        </table>
      </mat-card>
    </div>
  `,
  styles: [`
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    .page-title { margin: 0; font-weight: 500; color: #333; }
    .filter-card { margin-bottom: 20px; }
    .filters {
      display: flex;
      gap: 16px;
      align-items: center;
    }
    .search-field { flex: 1; }
    .product-table { width: 100%; }
    .product-name { font-weight: 500; }
    .product-generic { font-size: 12px; color: #888; }
    .stock-badge {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-weight: 600;
    }
    .stock-badge.danger { color: #e53935; }
    .low-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      color: #ff9800;
    }
  `]
})
export class InventoryComponent implements OnInit {
  products: Product[] = [];
  categories: string[] = [];
  search = '';
  selectedCategory = '';
  columns = ['name', 'category', 'manufacturer', 'costPrice', 'unitPrice', 'stock', 'expiry', 'actions'];

  constructor(
    private api: ApiService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadProducts();
    this.api.getCategories().subscribe(c => this.categories = c);
  }

  loadProducts() {
    this.api.getProducts(this.search, this.selectedCategory).subscribe(p => {
      this.products = p;
      this.cdr.detectChanges();
    });
  }

  openProductDialog(product?: Product) {
    const ref = this.dialog.open(ProductDialogComponent, {
      width: '600px',
      data: { product, categories: this.categories }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadProducts();
    });
  }

  deleteProduct(product: Product) {
    if (confirm(`Are you sure you want to delete "${product.name}"?`)) {
      this.api.deleteProduct(product.id).subscribe(() => {
        this.snack.open('Product deleted', 'OK', { duration: 3000 });
        this.loadProducts();
      });
    }
  }
}
