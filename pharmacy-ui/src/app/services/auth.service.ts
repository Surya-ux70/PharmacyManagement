import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponse, LoginRequest, RegisterRequest, RegisterTenantRequest, UserInfo } from '../models/pharmacy.models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = environment.apiUrl;
  private readonly TOKEN_KEY = 'pharma_token';
  private readonly USER_KEY = 'pharma_user';

  private currentUserSubject = new BehaviorSubject<UserInfo | null>(this.getStoredUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`, request).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  signUp(request: { fullName: string; email: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/signup`, request).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  googleSignIn(idToken: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/google`, { idToken }).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  registerTenant(request: RegisterTenantRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register-tenant`, request).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  register(request: RegisterRequest): Observable<UserInfo> {
    return this.http.post<UserInfo>(`${this.baseUrl}/auth/register`, request);
  }

  forgotPassword(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/auth/forgot-password`, { email });
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/auth/reset-password`, { email, token, newPassword });
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  get currentUser(): UserInfo | null {
    return this.currentUserSubject.value;
  }

  isAdmin(): boolean {
    return this.currentUser?.role === 'Admin';
  }

  isSuperAdmin(): boolean {
    return this.currentUser?.role === 'SuperAdmin';
  }

  hasTenant(): boolean {
    return this.currentUser?.tenantId != null;
  }

  get tenantName(): string | null {
    return this.currentUser?.tenantName ?? null;
  }

  private storeAuth(response: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    this.currentUserSubject.next(response.user);
  }

  private getStoredUser(): UserInfo | null {
    const stored = localStorage.getItem(this.USER_KEY);
    return stored ? JSON.parse(stored) : null;
  }
}
