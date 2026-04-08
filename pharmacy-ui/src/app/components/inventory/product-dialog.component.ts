import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Product } from '../../models/pharmacy.models';

@Component({
  selector: 'app-product-dialog',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatDatepickerModule,
    MatNativeDateModule, MatSnackBarModule
  ],
  template: `
    <h2 mat-dialog-title>{{ isEdit ? 'Edit' : 'Add' }} Product</h2>
    <mat-dialog-content>
      <div class="form-grid">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Product Name</mat-label>
          <input matInput [(ngModel)]="form.name" required>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Generic Name</mat-label>
          <input matInput [(ngModel)]="form.genericName">
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Category</mat-label>
          <input matInput [(ngModel)]="form.category">
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Manufacturer</mat-label>
          <input matInput [(ngModel)]="form.manufacturer">
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Cost Price</mat-label>
          <input matInput type="number" [(ngModel)]="form.costPrice" min="0" step="0.01">
          <span matTextPrefix>₹&nbsp;</span>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Selling Price</mat-label>
          <input matInput type="number" [(ngModel)]="form.unitPrice" min="0" step="0.01">
          <span matTextPrefix>₹&nbsp;</span>
        </mat-form-field>

        @if (!isEdit) {
          <mat-form-field appearance="outline">
            <mat-label>Initial Stock</mat-label>
            <input matInput type="number" [(ngModel)]="form.quantityInStock" min="0">
          </mat-form-field>
        }

        <mat-form-field appearance="outline">
          <mat-label>Reorder Level</mat-label>
          <input matInput type="number" [(ngModel)]="form.reorderLevel" min="0">
        </mat-form-field>

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
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" (click)="save()" [disabled]="!form.name">
        {{ isEdit ? 'Update' : 'Create' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0 16px;
    }
    .full-width { grid-column: 1 / -1; }
    mat-dialog-content { max-height: 70vh; }
  `]
})
export class ProductDialogComponent {
  isEdit: boolean;
  form: any = {
    name: '', genericName: '', category: '', manufacturer: '',
    unitPrice: 0, costPrice: 0, quantityInStock: 0, reorderLevel: 10,
    expiryDate: null, batchNumber: ''
  };

  constructor(
    private api: ApiService,
    private dialogRef: MatDialogRef<ProductDialogComponent>,
    private snack: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: { product?: Product; categories: string[] }
  ) {
    this.isEdit = !!data.product;
    if (data.product) {
      this.form = { ...data.product };
    }
  }

  save() {
    if (this.isEdit) {
      this.api.updateProduct(this.data.product!.id, this.form).subscribe(() => {
        this.snack.open('Product updated', 'OK', { duration: 3000 });
        this.dialogRef.close(true);
      });
    } else {
      this.api.createProduct(this.form).subscribe(() => {
        this.snack.open('Product created', 'OK', { duration: 3000 });
        this.dialogRef.close(true);
      });
    }
  }
}
