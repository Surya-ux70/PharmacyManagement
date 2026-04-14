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
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss'
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
