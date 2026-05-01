import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DatePipe } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { Tenant } from '../../models/pharmacy.models';

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [
    FormsModule, DatePipe,
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSlideToggleModule, MatChipsModule, MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './tenants.component.html',
  styleUrl: './tenants.component.scss'
})
export class TenantsComponent implements OnInit {
  tenants: Tenant[] = [];
  loading = true;
  showCreateForm = false;

  newTenant = { name: '', adminFullName: '', adminEmail: '', adminPassword: '' };
  creating = false;
  errorMessage = '';

  displayedColumns = ['id', 'name', 'slug', 'isActive', 'createdAt', 'actions'];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadTenants();
  }

  loadTenants(): void {
    this.loading = true;
    this.api.getTenants().subscribe({
      next: (tenants) => {
        this.tenants = tenants;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  onCreate(): void {
    this.creating = true;
    this.errorMessage = '';
    this.api.createTenant(this.newTenant).subscribe({
      next: () => {
        this.creating = false;
        this.showCreateForm = false;
        this.newTenant = { name: '', adminFullName: '', adminEmail: '', adminPassword: '' };
        this.loadTenants();
      },
      error: (err) => {
        this.creating = false;
        this.errorMessage = err.error?.message || 'Failed to create tenant.';
      }
    });
  }

  toggleActive(tenant: Tenant): void {
    this.api.updateTenant(tenant.id, { name: tenant.name, isActive: !tenant.isActive }).subscribe({
      next: (updated) => {
        const idx = this.tenants.findIndex(t => t.id === updated.id);
        if (idx >= 0) this.tenants[idx] = updated;
      }
    });
  }
}
