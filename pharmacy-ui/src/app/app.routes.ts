import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'inventory', loadComponent: () => import('./components/inventory/inventory.component').then(m => m.InventoryComponent) },
      { path: 'stock-entry', loadComponent: () => import('./components/stock-entry/stock-entry.component').then(m => m.StockEntryComponent) },
      { path: 'sales', loadComponent: () => import('./components/sales/sales.component').then(m => m.SalesComponent) },
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
