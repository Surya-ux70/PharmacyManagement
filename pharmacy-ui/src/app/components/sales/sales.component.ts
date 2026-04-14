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
  templateUrl: './sales.component.html',
  styleUrl: './sales.component.scss'
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
