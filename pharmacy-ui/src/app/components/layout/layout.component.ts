import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, RouterLinkActive,
    MatToolbarModule, MatSidenavModule, MatListModule,
    MatIconModule, MatButtonModule, MatBadgeModule,
    MatMenuModule, MatDividerModule
  ],
  template: `
    <div class="layout-container">
      <mat-toolbar color="primary" class="toolbar">
        <button mat-icon-button (click)="sidenav.toggle()">
          <mat-icon>menu</mat-icon>
        </button>
        <span class="brand">
          <mat-icon class="brand-icon">local_pharmacy</mat-icon>
          PharmaCare Manager
        </span>
        <span class="spacer"></span>
        @if (lowStockCount > 0) {
          <button mat-icon-button matBadge="{{ lowStockCount }}" matBadgeColor="warn"
                  routerLink="/inventory" [queryParams]="{filter: 'low-stock'}">
            <mat-icon>notifications</mat-icon>
          </button>
        }
        <button mat-icon-button [matMenuTriggerFor]="userMenu">
          <mat-icon>account_circle</mat-icon>
        </button>
        <mat-menu #userMenu="matMenu">
          <div class="user-info-menu">
            <strong>{{ auth.currentUser?.fullName }}</strong>
            <small>{{ auth.currentUser?.email }}</small>
            <span class="role-badge">{{ auth.currentUser?.role }}</span>
          </div>
          <mat-divider></mat-divider>
          @if (auth.isAdmin()) {
            <button mat-menu-item routerLink="/register">
              <mat-icon>person_add</mat-icon>
              <span>Register User</span>
            </button>
          }
          <button mat-menu-item (click)="auth.logout()">
            <mat-icon>logout</mat-icon>
            <span>Sign Out</span>
          </button>
        </mat-menu>
      </mat-toolbar>

      <mat-sidenav-container class="sidenav-container">
        <mat-sidenav #sidenav mode="side" opened class="sidenav">
          <mat-nav-list>
            <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link">
              <mat-icon matListItemIcon>dashboard</mat-icon>
              <span matListItemTitle>Dashboard</span>
            </a>
            <a mat-list-item routerLink="/inventory" routerLinkActive="active-link">
              <mat-icon matListItemIcon>inventory_2</mat-icon>
              <span matListItemTitle>Inventory</span>
            </a>
            <a mat-list-item routerLink="/stock-entry" routerLinkActive="active-link">
              <mat-icon matListItemIcon>add_shopping_cart</mat-icon>
              <span matListItemTitle>Stock Entry</span>
            </a>
            <a mat-list-item routerLink="/sales" routerLinkActive="active-link">
              <mat-icon matListItemIcon>point_of_sale</mat-icon>
              <span matListItemTitle>Sales</span>
            </a>
            @if (auth.isAdmin()) {
              <mat-divider></mat-divider>
              <a mat-list-item routerLink="/register" routerLinkActive="active-link">
                <mat-icon matListItemIcon>person_add</mat-icon>
                <span matListItemTitle>Register User</span>
              </a>
            }
          </mat-nav-list>
        </mat-sidenav>
        <mat-sidenav-content class="content">
          <router-outlet />
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .layout-container {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }
    .toolbar {
      position: fixed;
      top: 0;
      z-index: 1000;
    }
    .brand {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 20px;
      font-weight: 500;
    }
    .brand-icon { margin-right: 4px; }
    .spacer { flex: 1; }
    .sidenav-container {
      flex: 1;
      margin-top: 64px;
    }
    .sidenav {
      width: 240px;
      border-right: 1px solid rgba(0, 0, 0, 0.08);
    }
    .content {
      padding: 24px;
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }
    .active-link {
      background: rgba(63, 81, 181, 0.08) !important;
      color: #3f51b5;
    }
    .user-info-menu {
      display: flex;
      flex-direction: column;
      padding: 12px 16px;
      gap: 2px;
    }
    .user-info-menu small {
      color: rgba(0, 0, 0, 0.54);
      font-size: 12px;
    }
    .role-badge {
      display: inline-block;
      margin-top: 4px;
      padding: 2px 8px;
      background: #e8eaf6;
      color: #3f51b5;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 500;
      width: fit-content;
    }
  `]
})
export class LayoutComponent {
  lowStockCount = 0;

  constructor(public auth: AuthService, private api: ApiService) {
    this.api.getLowStockAlerts().subscribe(alerts => {
      this.lowStockCount = alerts.length;
    });
  }
}
