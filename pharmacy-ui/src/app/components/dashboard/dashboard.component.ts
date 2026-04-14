import { Component, OnInit, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
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
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, AfterViewChecked {
  data: Dashboard | null = null;
  loading = true;
  private chartsRendered = false;
  lowStockColumns = ['name', 'category', 'stock', 'reorder'];

  @ViewChild('revenueChart') revenueChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('categoryChart') categoryChartRef!: ElementRef<HTMLCanvasElement>;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getDashboard().subscribe({
      next: data => {
        this.data = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Dashboard API error:', err);
        this.loading = false;
      }
    });
  }

  ngAfterViewChecked() {
    if (this.data && !this.chartsRendered && this.revenueChartRef && this.categoryChartRef) {
      this.chartsRendered = true;
      this.renderCharts();
    }
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
