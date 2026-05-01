import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register-tenant',
  standalone: true,
  imports: [
    FormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  templateUrl: './register-tenant.component.html',
  styleUrl: './register-tenant.component.scss'
})
export class RegisterTenantComponent {
  pharmacyName = '';
  fullName = '';
  email = '';
  password = '';
  hidePassword = true;
  loading = false;
  errorMessage = '';

  constructor(private auth: AuthService, private router: Router) {
    if (this.auth.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onRegister(): void {
    this.loading = true;
    this.errorMessage = '';

    this.auth.registerTenant({
      pharmacyName: this.pharmacyName,
      fullName: this.fullName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.loading = false;
        const errors = err.error?.errors;
        this.errorMessage = errors ? errors.join(' ') : (err.error?.message || 'Registration failed.');
      }
    });
  }
}
