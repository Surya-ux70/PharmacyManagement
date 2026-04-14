import { Component, NgZone } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';

declare const google: any;

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    FormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatDividerModule
  ],
  template: `
    <div class="signup-wrapper">
      <mat-card class="signup-card">
        <mat-card-header>
          <div class="header-content">
            <mat-icon class="brand-icon">local_pharmacy</mat-icon>
            <mat-card-title>Create Account</mat-card-title>
            <mat-card-subtitle>Join PharmaCare Manager</mat-card-subtitle>
          </div>
        </mat-card-header>

        <mat-card-content>
          @if (errorMessage) {
            <div class="error-banner">
              <mat-icon>error_outline</mat-icon>
              {{ errorMessage }}
            </div>
          }

          <div id="google-signup-btn" class="google-btn-container"></div>

          <div class="divider-row">
            <mat-divider></mat-divider>
            <span class="divider-text">or sign up with email</span>
            <mat-divider></mat-divider>
          </div>

          <form (ngSubmit)="onSignUp()" #signupForm="ngForm">
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
              <mat-hint>Min 6 chars, uppercase, lowercase, and digit</mat-hint>
            </mat-form-field>

            <button mat-raised-button color="primary" type="submit"
                    class="full-width signup-btn" [disabled]="loading || !signupForm.valid">
              @if (loading) {
                <mat-spinner diameter="20"></mat-spinner>
              } @else {
                Create Account
              }
            </button>
          </form>

          <p class="footer-text">
            Already have an account? <a routerLink="/login">Sign in</a>
          </p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .signup-wrapper {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }
    .signup-card {
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
    .signup-btn {
      height: 48px;
      font-size: 16px;
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
    .google-btn-container {
      display: flex;
      justify-content: center;
      margin-bottom: 16px;
    }
    .divider-row {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 16px;
    }
    .divider-row mat-divider { flex: 1; }
    .divider-text {
      color: rgba(0, 0, 0, 0.54);
      font-size: 13px;
      white-space: nowrap;
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
export class SignUpComponent {
  fullName = '';
  email = '';
  password = '';
  hidePassword = true;
  loading = false;
  errorMessage = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private ngZone: NgZone
  ) {
    if (this.auth.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngAfterViewInit(): void {
    if (typeof google !== 'undefined' && environment.googleClientId) {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: (response: any) => this.handleGoogleSignIn(response)
      });
      google.accounts.id.renderButton(
        document.getElementById('google-signup-btn'),
        { theme: 'outline', size: 'large', width: 356, text: 'signup_with' }
      );
    }
  }

  handleGoogleSignIn(response: any): void {
    this.ngZone.run(() => {
      this.loading = true;
      this.errorMessage = '';
      this.auth.googleSignIn(response.credential).subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: (err) => {
          this.loading = false;
          this.errorMessage = err.error?.message || 'Google sign-in failed.';
        }
      });
    });
  }

  onSignUp(): void {
    this.loading = true;
    this.errorMessage = '';

    this.auth.signUp({
      fullName: this.fullName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.loading = false;
        const errors = err.error?.errors;
        this.errorMessage = errors ? errors.join(' ') : (err.error?.message || 'Sign up failed.');
      }
    });
  }
}
