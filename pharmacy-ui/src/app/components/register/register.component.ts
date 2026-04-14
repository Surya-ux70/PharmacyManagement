import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatSelectModule,
    MatProgressSpinnerModule, MatSnackBarModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  fullName = '';
  email = '';
  password = '';
  role = 'Pharmacist';
  hidePassword = true;
  loading = false;
  errorMessage = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onRegister(): void {
    this.loading = true;
    this.errorMessage = '';

    this.auth.register({
      fullName: this.fullName,
      email: this.email,
      password: this.password,
      role: this.role
    }).subscribe({
      next: (user) => {
        this.snackBar.open(`User "${user.fullName}" created successfully!`, 'Close', { duration: 3000 });
        this.resetForm();
      },
      error: (err) => {
        this.loading = false;
        const errors = err.error?.errors;
        this.errorMessage = errors ? errors.join(' ') : (err.error?.message || 'Registration failed.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  private resetForm(): void {
    this.fullName = '';
    this.email = '';
    this.password = '';
    this.role = 'Pharmacist';
    this.loading = false;
  }
}
