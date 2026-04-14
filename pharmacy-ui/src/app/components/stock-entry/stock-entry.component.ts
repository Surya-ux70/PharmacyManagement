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
  templateUrl: './stock-entry.component.html',
  styleUrl: './stock-entry.component.scss'
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
