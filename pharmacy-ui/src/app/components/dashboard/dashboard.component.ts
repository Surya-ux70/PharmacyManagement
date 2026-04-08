import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../../services/api.service';
import { Dashboard } from '../../models/pharmacy.models';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe,
    MatCardModule, MatIconModule, MatTableModule,
    MatChipsModule, MatProgressSpinnerModule
  ],
  template: `
    <div class="dashboard">
      <h1 class="page-title">Financial Dashboard</h1>

      @if (loading) {
        <div class="loading">
          <mat-spinner diameter="48"></mat-spinner>
        </div>
      } @else if (data) {
        <div class="kpi-grid">
          <mat-card class="kpi-card revenue">
            <mat-card-content>
              <div class="kpi-icon"><mat-icon>trending_up</mat-icon></div>
              <div class="kpi-info">
                <span class="kpi-label">Total Revenue</span>
                <span class="kpi-value">{{ data.totalRevenue | currency:'INR' }}</span>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card cost">
            <mat-card-content>
              <div class="kpi-icon"><mat-icon>shopping_cart</mat-icon></div>
              <div class="kpi-info">
                <span class="kpi-label">Cost of Goods Sold</span>
                <span class="kpi-value">{{ data.totalCost | currency:'INR' }}</span>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card profit">
            <mat-card-content>
              <div class="kpi-icon"><mat-icon>account_balance_wallet</mat-icon></div>
              <div class="kpi-info">
                <span class="kpi-label">Net Profit</span>
                <span class="kpi-value">{{ data.netProfit | currency:'INR' }}</span>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="kpi-card margin">
            <mat-card-content>
              <div class="kpi-icon"><mat-icon>percent</mat-icon></div>
              <div class="kpi-info">
                <span class="kpi-label">Profit Margin</span>
                <span class="kpi-value">{{ data.profitMargin | number:'1.1-1' }}%</span>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <div class="stats-row">
          <mat-card class="stat-card">
            <mat-card-content>
              <mat-icon>medication</mat-icon>
              <span class="stat-value">{{ data.totalProducts }}</span>
              <span class="stat-label">Products</span>
            </mat-card-content>
          </mat-card>
          <mat-card class="stat-card">
            <mat-card-content>
              <mat-icon>receipt_long</mat-icon>
              <span class="stat-value">{{ data.totalSales }}</span>
              <span class="stat-label">Total Sales</span>
            </mat-card-content>
          </mat-card>
          <mat-card class="stat-card" [class.alert]="data.lowStockCount > 0">
            <mat-card-content>
              <mat-icon>warning</mat-icon>
              <span class="stat-value">{{ data.lowStockCount }}</span>
              <span class="stat-label">Low Stock Items</span>
            </mat-card-content>
          </mat-card>
        </div>

        <div class="charts-row">
          <mat-card class="chart-card">
            <mat-card-header><mat-card-title>Revenue vs Cost vs Profit</mat-card-title></mat-card-header>
            <mat-card-content>
              <canvas #revenueChart></canvas>
            </mat-card-content>
          </mat-card>

          <mat-card class="chart-card">
            <mat-card-header><mat-card-title>Revenue by Category</mat-card-title></mat-card-header>
            <mat-card-content>
              <canvas #categoryChart></canvas>
            </mat-card-content>
          </mat-card>
        </div>

        @if (data.lowStockProducts.length > 0) {
          <mat-card class="low-stock-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon class="warn-icon">warning</mat-icon>
                Low Stock Alerts
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <table mat-table [dataSource]="data.lowStockProducts" class="low-stock-table">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Product</th>
                  <td mat-cell *matCellDef="let p">{{ p.name }}</td>
                </ng-container>
                <ng-container matColumnDef="category">
                  <th mat-header-cell *matHeaderCellDef>Category</th>
                  <td mat-cell *matCellDef="let p">
                    <mat-chip>{{ p.category }}</mat-chip>
                  </td>
                </ng-container>
                <ng-container matColumnDef="stock">
                  <th mat-header-cell *matHeaderCellDef>Current Stock</th>
                  <td mat-cell *matCellDef="let p" class="warn-text">{{ p.quantityInStock }}</td>
                </ng-container>
                <ng-container matColumnDef="reorder">
                  <th mat-header-cell *matHeaderCellDef>Reorder Level</th>
                  <td mat-cell *matCellDef="let p">{{ p.reorderLevel }}</td>
                </ng-container>
                <tr mat-header-row *matHeaderRowDef="lowStockColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: lowStockColumns;"></tr>
              </table>
            </mat-card-content>
          </mat-card>
        }
      }
    </div>
  `,
  styles: [`
    .page-title { margin: 0 0 24px; font-weight: 500; color: #333; }
    .loading { display: flex; justify-content: center; padding: 80px 0; }

    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
      gap: 20px;
      margin-bottom: 24px;
    }
    .kpi-card mat-card-content {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 20px !important;
    }
    .kpi-icon {
      width: 56px; height: 56px;
      border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
    }
    .kpi-icon mat-icon { font-size: 28px; width: 28px; height: 28px; color: #fff; }
    .revenue .kpi-icon { background: linear-gradient(135deg, #43a047, #66bb6a); }
    .cost .kpi-icon { background: linear-gradient(135deg, #e53935, #ef5350); }
    .profit .kpi-icon { background: linear-gradient(135deg, #1e88e5, #42a5f5); }
    .margin .kpi-icon { background: linear-gradient(135deg, #8e24aa, #ab47bc); }
    .kpi-info { display: flex; flex-direction: column; }
    .kpi-label { font-size: 13px; color: #888; text-transform: uppercase; letter-spacing: 0.5px; }
    .kpi-value { font-size: 28px; font-weight: 700; color: #333; }

    .stats-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 16px;
      margin-bottom: 24px;
    }
    .stat-card mat-card-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 20px !important;
      gap: 8px;
    }
    .stat-card mat-icon { font-size: 32px; width: 32px; height: 32px; color: #3f51b5; }
    .stat-value { font-size: 32px; font-weight: 700; color: #333; }
    .stat-label { font-size: 13px; color: #888; text-transform: uppercase; }
    .stat-card.alert { border-left: 4px solid #ff9800; }
    .stat-card.alert mat-icon { color: #ff9800; }

    .charts-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
      margin-bottom: 24px;
    }
    .chart-card { padding: 16px; }
    .chart-card canvas { max-height: 300px; }

    .low-stock-card { margin-bottom: 24px; }
    .low-stock-table { width: 100%; }
    .warn-icon { color: #ff9800; vertical-align: middle; margin-right: 8px; }
    .warn-text { color: #e53935; font-weight: 600; }

    @media (max-width: 768px) {
      .charts-row { grid-template-columns: 1fr; }
      .kpi-grid { grid-template-columns: repeat(2, 1fr); }
    }
  `]
})
export class DashboardComponent implements OnInit {
  data: Dashboard | null = null;
  loading = true;
  lowStockColumns = ['name', 'category', 'stock', 'reorder'];

  @ViewChild('revenueChart') revenueChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('categoryChart') categoryChartRef!: ElementRef<HTMLCanvasElement>;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getDashboard().subscribe(data => {
      this.data = data;
      this.loading = false;
      setTimeout(() => this.renderCharts(), 0);
    });
  }

  private renderCharts() {
    if (!this.data) return;

    const months = this.data.monthlySummaries.map(m => m.month);
    new Chart(this.revenueChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: months,
        datasets: [
          { label: 'Revenue', data: this.data.monthlySummaries.map(m => m.revenue), backgroundColor: 'rgba(67, 160, 71, 0.7)', borderRadius: 6 },
          { label: 'COGS', data: this.data.monthlySummaries.map(m => m.cost), backgroundColor: 'rgba(229, 57, 53, 0.7)', borderRadius: 6 },
          { label: 'Profit', data: this.data.monthlySummaries.map(m => m.profit), backgroundColor: 'rgba(30, 136, 229, 0.7)', borderRadius: 6 }
        ]
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'bottom' } },
        scales: { y: { beginAtZero: true, ticks: { callback: v => '₹' + v } } }
      }
    });

    const categories = this.data.categorySummaries.map(c => c.category);
    const palette = ['#3f51b5', '#e91e63', '#00bcd4', '#ff9800', '#4caf50', '#9c27b0', '#795548', '#607d8b'];
    new Chart(this.categoryChartRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels: categories,
        datasets: [{
          data: this.data.categorySummaries.map(c => c.revenue),
          backgroundColor: palette.slice(0, categories.length),
          borderWidth: 2, borderColor: '#fff'
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom' },
          tooltip: { callbacks: { label: ctx => `${ctx.label}: ₹${ctx.parsed.toFixed(2)}` } }
        }
      }
    });
  }
}
