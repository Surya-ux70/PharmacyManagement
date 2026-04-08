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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService } from '../../services/api.service';
import { Product, StockEntry } from '../../models/pharmacy.models';

@Component({
  selector: 'app-stock-entry',
  standalone: true,
  imports: [
    CommonModule, FormsModule, CurrencyPipe, DatePipe,
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatInputModule, MatFormFieldModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule, MatSnackBarModule,
    MatDividerModule
  ],
  template: `
    <div class="stock-entry">
      <h1 class="page-title">Stock Entry</h1>

      <div class="content-grid">
        <mat-card class="form-card">
          <mat-card-header>
            <mat-card-title>Receive New Stock</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="form-stack">
              <mat-form-field appearance="outline">
                <mat-label>Product</mat-label>
                <mat-select [(ngModel)]="form.productId" required>
                  @for (p of products; track p.id) {
                    <mat-option [value]="p.id">{{ p.name }} (Stock: {{ p.quantityInStock }})</mat-option>
                  }
                </mat-select>
              </mat-form-field>

              <div class="row">
                <mat-form-field appearance="outline">
                  <mat-label>Quantity</mat-label>
                  <input matInput type="number" [(ngModel)]="form.quantity" min="1" required>
                </mat-form-field>

                <mat-form-field appearance="outline">
                  <mat-label>Cost per Unit</mat-label>
                  <input matInput type="number" [(ngModel)]="form.costPricePerUnit" min="0" step="0.01">
                  <span matTextPrefix>₹&nbsp;</span>
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline">
                <mat-label>Supplier</mat-label>
                <input matInput [(ngModel)]="form.supplier">
              </mat-form-field>

              <div class="row">
                <mat-form-field appearance="outline">
                  <mat-label>Batch Number</mat-label>
                  <input matInput [(ngModel)]="form.batchNumber">
                </mat-form-field>

                <mat-form-field appearance="outline">
                  <mat-label>Expiry Date</mat-label>
                  <input matInput [matDatepicker]="picker" [(ngModel)]="form.expiryDate">
                  <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                  <mat-datepicker #picker></mat-datepicker>
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline">
                <mat-label>Notes</mat-label>
                <textarea matInput [(ngModel)]="form.notes" rows="2"></textarea>
              </mat-form-field>

              <button mat-raised-button color="primary" (click)="submitEntry()"
                      [disabled]="!form.productId || !form.quantity">
                <mat-icon>add_shopping_cart</mat-icon> Record Stock Entry
              </button>
            </div>
          </mat-card-content>
        </mat-card>

        <mat-card>
          <mat-card-header>
            <mat-card-title>Recent Stock Entries</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <table mat-table [dataSource]="entries" class="entries-table">
              <ng-container matColumnDef="date">
                <th mat-header-cell *matHeaderCellDef>Date</th>
                <td mat-cell *matCellDef="let e">{{ e.entryDate | date:'mediumDate' }}</td>
              </ng-container>
              <ng-container matColumnDef="product">
                <th mat-header-cell *matHeaderCellDef>Product</th>
                <td mat-cell *matCellDef="let e">{{ e.productName }}</td>
              </ng-container>
              <ng-container matColumnDef="quantity">
                <th mat-header-cell *matHeaderCellDef>Qty</th>
                <td mat-cell *matCellDef="let e" class="qty">+{{ e.quantity }}</td>
              </ng-container>
              <ng-container matColumnDef="cost">
                <th mat-header-cell *matHeaderCellDef>Unit Cost</th>
                <td mat-cell *matCellDef="let e">{{ e.costPricePerUnit | currency:'INR' }}</td>
              </ng-container>
              <ng-container matColumnDef="supplier">
                <th mat-header-cell *matHeaderCellDef>Supplier</th>
                <td mat-cell *matCellDef="let e">{{ e.supplier }}</td>
              </ng-container>
              <ng-container matColumnDef="total">
                <th mat-header-cell *matHeaderCellDef>Total Cost</th>
                <td mat-cell *matCellDef="let e">{{ e.quantity * e.costPricePerUnit | currency:'INR' }}</td>
              </ng-container>
              <tr mat-header-row *matHeaderRowDef="entryColumns"></tr>
              <tr mat-row *matRowDef="let row; columns: entryColumns;"></tr>
            </table>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .page-title { margin: 0 0 24px; font-weight: 500; color: #333; }
    .content-grid {
      display: grid;
      grid-template-columns: 400px 1fr;
      gap: 24px;
      align-items: start;
    }
    .form-stack {
      display: flex;
      flex-direction: column;
      gap: 4px;
      padding-top: 16px;
    }
    .form-stack mat-form-field { width: 100%; }
    .row { display: flex; gap: 16px; }
    .row mat-form-field { flex: 1; }
    .entries-table { width: 100%; }
    .qty { color: #43a047; font-weight: 600; }
    @media (max-width: 900px) {
      .content-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class StockEntryComponent implements OnInit {
  products: Product[] = [];
  entries: StockEntry[] = [];
  entryColumns = ['date', 'product', 'quantity', 'cost', 'supplier', 'total'];
  form: any = {
    productId: null, quantity: 1, costPricePerUnit: 0,
    supplier: '', batchNumber: '', expiryDate: null, notes: ''
  };

  constructor(private api: ApiService, private snack: MatSnackBar) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.api.getProducts().subscribe(p => this.products = p);
    this.api.getStockEntries().subscribe(e => this.entries = e);
  }

  submitEntry() {
    this.api.createStockEntry(this.form).subscribe(() => {
      this.snack.open('Stock entry recorded', 'OK', { duration: 3000 });
      this.form = {
        productId: null, quantity: 1, costPricePerUnit: 0,
        supplier: '', batchNumber: '', expiryDate: null, notes: ''
      };
      this.loadData();
    });
  }
}
