import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '../../services/api.service';
import { Product, Sale, CreateSaleItem } from '../../models/pharmacy.models';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [
    CommonModule, FormsModule, CurrencyPipe, DatePipe,
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatInputModule, MatFormFieldModule, MatSelectModule,
    MatSnackBarModule, MatExpansionModule, MatDividerModule, MatChipsModule
  ],
  template: `
    <div class="sales">
      <h1 class="page-title">Sales</h1>

      <div class="content-grid">
        <mat-card class="form-card">
          <mat-card-header>
            <mat-card-title>New Sale</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="form-stack">
              <mat-form-field appearance="outline">
                <mat-label>Customer Name</mat-label>
                <input matInput [(ngModel)]="customerName">
              </mat-form-field>

              <div class="item-header">
                <strong>Sale Items</strong>
                <button mat-mini-fab color="primary" (click)="addItem()">
                  <mat-icon>add</mat-icon>
                </button>
              </div>

              @for (item of saleItems; track $index; let i = $index) {
                <div class="sale-item">
                  <mat-form-field appearance="outline" class="product-select">
                    <mat-label>Product</mat-label>
                    <mat-select [(ngModel)]="item.productId" (selectionChange)="updateTotal()">
                      @for (p of products; track p.id) {
                        <mat-option [value]="p.id">{{ p.name }} ({{ p.quantityInStock }} avail.)</mat-option>
                      }
                    </mat-select>
                  </mat-form-field>
                  <mat-form-field appearance="outline" class="qty-field">
                    <mat-label>Qty</mat-label>
                    <input matInput type="number" [(ngModel)]="item.quantity" min="1" (input)="updateTotal()">
                  </mat-form-field>
                  <button mat-icon-button color="warn" (click)="removeItem(i)">
                    <mat-icon>close</mat-icon>
                  </button>
                </div>
              }

              <mat-divider></mat-divider>

              <div class="total-row">
                <span>Estimated Total:</span>
                <strong>{{ estimatedTotal | currency:'INR' }}</strong>
              </div>

              <button mat-raised-button color="primary" (click)="submitSale()"
                      [disabled]="saleItems.length === 0 || !saleItems[0].productId">
                <mat-icon>point_of_sale</mat-icon> Complete Sale
              </button>
            </div>
          </mat-card-content>
        </mat-card>

        <mat-card>
          <mat-card-header>
            <mat-card-title>Sales History</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-accordion>
              @for (sale of sales; track sale.id) {
                <mat-expansion-panel>
                  <mat-expansion-panel-header>
                    <mat-panel-title>
                      <span class="invoice">{{ sale.invoiceNumber }}</span>
                    </mat-panel-title>
                    <mat-panel-description>
                      <span>{{ sale.customerName }}</span>
                      <span class="sale-date">{{ sale.saleDate | date:'mediumDate' }}</span>
                      <mat-chip class="sale-total">{{ sale.totalAmount | currency:'INR' }}</mat-chip>
                    </mat-panel-description>
                  </mat-expansion-panel-header>
                  <table mat-table [dataSource]="sale.items" class="items-table">
                    <ng-container matColumnDef="product">
                      <th mat-header-cell *matHeaderCellDef>Product</th>
                      <td mat-cell *matCellDef="let i">{{ i.productName }}</td>
                    </ng-container>
                    <ng-container matColumnDef="qty">
                      <th mat-header-cell *matHeaderCellDef>Qty</th>
                      <td mat-cell *matCellDef="let i">{{ i.quantity }}</td>
                    </ng-container>
                    <ng-container matColumnDef="price">
                      <th mat-header-cell *matHeaderCellDef>Unit Price</th>
                      <td mat-cell *matCellDef="let i">{{ i.unitPrice | currency:'INR' }}</td>
                    </ng-container>
                    <ng-container matColumnDef="subtotal">
                      <th mat-header-cell *matHeaderCellDef>Subtotal</th>
                      <td mat-cell *matCellDef="let i">{{ i.subTotal | currency:'INR' }}</td>
                    </ng-container>
                    <tr mat-header-row *matHeaderRowDef="itemColumns"></tr>
                    <tr mat-row *matRowDef="let row; columns: itemColumns;"></tr>
                  </table>
                  <div class="sale-summary">
                    <span>Revenue: {{ sale.totalAmount | currency:'INR' }}</span>
                    <span>Cost: {{ sale.totalCost | currency:'INR' }}</span>
                    <span class="profit">Profit: {{ sale.profit | currency:'INR' }}</span>
                  </div>
                </mat-expansion-panel>
              }
            </mat-accordion>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .page-title { margin: 0 0 24px; font-weight: 500; color: #333; }
    .content-grid {
      display: grid;
      grid-template-columns: 420px 1fr;
      gap: 24px;
      align-items: start;
    }
    .form-stack {
      display: flex;
      flex-direction: column;
      gap: 8px;
      padding-top: 16px;
    }
    .form-stack mat-form-field { width: 100%; }
    .item-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin: 8px 0;
    }
    .sale-item {
      display: flex;
      gap: 8px;
      align-items: center;
    }
    .product-select { flex: 2; }
    .qty-field { flex: 0.6; }
    .total-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 0;
      font-size: 18px;
    }
    .invoice { font-weight: 600; color: #3f51b5; }
    .sale-date { margin-left: auto; margin-right: 12px; color: #888; }
    .items-table { width: 100%; }
    .sale-summary {
      display: flex;
      gap: 24px;
      padding: 12px 0;
      font-size: 14px;
    }
    .profit { color: #43a047; font-weight: 600; }
    @media (max-width: 900px) {
      .content-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class SalesComponent implements OnInit {
  products: Product[] = [];
  sales: Sale[] = [];
  customerName = '';
  saleItems: CreateSaleItem[] = [{ productId: 0, quantity: 1 }];
  estimatedTotal = 0;
  itemColumns = ['product', 'qty', 'price', 'subtotal'];

  constructor(private api: ApiService, private snack: MatSnackBar) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.api.getProducts().subscribe(p => this.products = p);
    this.api.getSales().subscribe(s => this.sales = s);
  }

  addItem() { this.saleItems.push({ productId: 0, quantity: 1 }); }
  removeItem(i: number) {
    this.saleItems.splice(i, 1);
    this.updateTotal();
  }

  updateTotal() {
    this.estimatedTotal = this.saleItems.reduce((sum, item) => {
      const product = this.products.find(p => p.id === item.productId);
      return sum + (product ? product.unitPrice * item.quantity : 0);
    }, 0);
  }

  submitSale() {
    const items = this.saleItems.filter(i => i.productId > 0 && i.quantity > 0);
    if (items.length === 0) return;

    this.api.createSale({ customerName: this.customerName || 'Walk-in', items }).subscribe({
      next: () => {
        this.snack.open('Sale completed successfully', 'OK', { duration: 3000 });
        this.customerName = '';
        this.saleItems = [{ productId: 0, quantity: 1 }];
        this.estimatedTotal = 0;
        this.loadData();
      },
      error: (err) => {
        this.snack.open(err.error || 'Sale failed', 'OK', { duration: 5000 });
      }
    });
  }
}
