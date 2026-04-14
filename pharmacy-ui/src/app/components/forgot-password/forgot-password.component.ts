import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    FormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  template: `
    <div class="forgot-wrapper">
      <mat-card class="forgot-card">
        <mat-card-header>
          <div class="header-content">
            <mat-icon class="brand-icon">lock_reset</mat-icon>
            <mat-card-title>Forgot Password</mat-card-title>
            <mat-card-subtitle>Enter your email to receive a reset link</mat-card-subtitle>
          </div>
        </mat-card-header>

        <mat-card-content>
          @if (submitted) {
            <div class="success-banner">
              <mat-icon>check_circle_outline</mat-icon>
              If an account exists with that email, a password reset link has been sent.
            </div>
            <button mat-stroked-button routerLink="/login" class="full-width back-btn">
              <mat-icon>arrow_back</mat-icon>
              Back to Sign In
            </button>
          } @else {
            @if (errorMessage) {
              <div class="error-banner">
                <mat-icon>error_outline</mat-icon>
                {{ errorMessage }}
              </div>
            }

            <form (ngSubmit)="onSubmit()" #forgotForm="ngForm">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Email</mat-label>
                <input matInput type="email" name="email" [(ngModel)]="email" required email>
                <mat-icon matPrefix>email</mat-icon>
              </mat-form-field>

              <button mat-raised-button color="primary" type="submit"
                      class="full-width submit-btn" [disabled]="loading || !forgotForm.valid">
                @if (loading) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Send Reset Link
                }
              </button>
            </form>

            <p class="footer-text">
              Remember your password? <a routerLink="/login">Sign in</a>
            </p>
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .forgot-wrapper {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }
    .forgot-card {
      width: 100%;
      max-width: 420px;
      padding: 32px;
      border-radius: 16px;
    }
    .header-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      width: 100%;
      margin-bottom: 24px;
    }
    .brand-icon {
      font-size: 48px;
      height: 48px;
      width: 48px;
      color: #3f51b5;
      margin-bottom: 8px;
    }
    mat-card-title { font-size: 24px !important; text-align: center; }
    mat-card-subtitle { text-align: center; margin-top: 4px; }
    .full-width { width: 100%; }
    .submit-btn {
      height: 48px;
      font-size: 16px;
      margin-top: 8px;
    }
    .back-btn {
      height: 48px;
      margin-top: 8px;
    }
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
    .success-banner {
      display: flex;
      align-items: center;
      gap: 8px;
      background: #e8f5e9;
      color: #2e7d32;
      padding: 12px 16px;
      border-radius: 8px;
      margin-bottom: 16px;
      font-size: 14px;
    }
    .footer-text {
      text-align: center;
      margin-top: 16px;
      color: rgba(0, 0, 0, 0.6);
    }
    .footer-text a { color: #3f51b5; text-decoration: none; font-weight: 500; }
    mat-card-header { display: flex; justify-content: center; }
  `]
})
export class ForgotPasswordComponent {
  email = '';
  loading = false;
  submitted = false;
  errorMessage = '';

  constructor(private auth: AuthService) {}

  onSubmit(): void {
    this.loading = true;
    this.errorMessage = '';

    this.auth.forgotPassword(this.email).subscribe({
      next: () => {
        this.submitted = true;
        this.loading = false;
      },
      error: () => {
        this.submitted = true;
        this.loading = false;
      }
    });
  }
}
