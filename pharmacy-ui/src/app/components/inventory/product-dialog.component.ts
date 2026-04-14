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
  templateUrl: './product-dialog.component.html',
  styleUrl: './product-dialog.component.scss'
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
