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
  template: `
    <div class="register-container">
      <mat-card class="register-card">
        <mat-card-header>
          <mat-card-title>
            <mat-icon class="title-icon">person_add</mat-icon>
            Register New User
          </mat-card-title>
          <mat-card-subtitle>Create accounts for your pharmacy staff</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          @if (errorMessage) {
            <div class="error-banner">
              <mat-icon>error_outline</mat-icon>
              {{ errorMessage }}
            </div>
          }

          <form (ngSubmit)="onRegister()" #registerForm="ngForm">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Full Name</mat-label>
              <input matInput name="fullName" [(ngModel)]="fullName" required>
              <mat-icon matPrefix>person</mat-icon>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" name="email" [(ngModel)]="email" required email>
              <mat-icon matPrefix>email</mat-icon>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword ? 'password' : 'text'"
                     name="password" [(ngModel)]="password" required minlength="6">
              <mat-icon matPrefix>lock</mat-icon>
              <button mat-icon-button matSuffix type="button"
                      (click)="hidePassword = !hidePassword">
                <mat-icon>{{ hidePassword ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              <mat-hint>Min 6 chars, uppercase, lowercase, and digit required</mat-hint>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Role</mat-label>
              <mat-select name="role" [(ngModel)]="role" required>
                <mat-option value="Pharmacist">Pharmacist</mat-option>
                <mat-option value="Admin">Admin</mat-option>
              </mat-select>
              <mat-icon matPrefix>badge</mat-icon>
            </mat-form-field>

            <div class="actions">
              <button mat-stroked-button type="button" (click)="goBack()">
                Cancel
              </button>
              <button mat-raised-button color="primary" type="submit"
                      [disabled]="loading || !registerForm.valid">
                @if (loading) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Create User
                }
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .register-container {
      max-width: 520px;
      margin: 0 auto;
    }
    .register-card {
      padding: 24px;
      border-radius: 12px;
    }
    .title-icon {
      vertical-align: middle;
      margin-right: 8px;
    }
    .full-width { width: 100%; }
    .error-banner {
      display: flex;
      align-items: center;
      gap: 8px;
      background: #fdecea;
      color: #d32f2f;
      padding: 12px 16px;
      border-radius: 8px;
      margin-bottom: 16px;
      font-size: 14px;
    }
    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 16px;
    }
  `]
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
