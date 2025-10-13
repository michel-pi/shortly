import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatsApi, ShortLinkEngagementSummaryResponse } from '../../api/stats.api';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';

type KV = { key: string; value: number };

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule, MatCardModule, MatTableModule],
  template: `
    <div class="grid">
      <mat-card class="kpi">
        <h3>Total Clicks</h3>
        <div class="num">{{ clicks() }}</div>
      </mat-card>

      <mat-card class="kpi">
        <h3>Total Clients</h3>
        <div class="num">{{ clients() }}</div>
      </mat-card>
    </div>

    <div class="tables">
      <mat-card>
        <h3>Top Referrers</h3>
        <table mat-table [dataSource]="referrers()">
          <ng-container matColumnDef="key">
            <th mat-header-cell *matHeaderCellDef>Referrer</th>
            <td mat-cell *matCellDef="let r">{{ r.key || '—' }}</td>
          </ng-container>
          <ng-container matColumnDef="value">
            <th mat-header-cell *matHeaderCellDef>Clicks</th>
            <td mat-cell *matCellDef="let r">{{ r.value }}</td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols;"></tr>
        </table>
      </mat-card>

      <mat-card>
        <h3>Top Countries</h3>
        <table mat-table [dataSource]="countries()">
          <ng-container matColumnDef="key">
            <th mat-header-cell *matHeaderCellDef>Country</th>
            <td mat-cell *matCellDef="let r">{{ r.key || '—' }}</td>
          </ng-container>
          <ng-container matColumnDef="value">
            <th mat-header-cell *matHeaderCellDef>Clicks</th>
            <td mat-cell *matCellDef="let r">{{ r.value }}</td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols;"></tr>
        </table>
      </mat-card>
    </div>
  `,
  styles: [`
    .grid { display:grid; grid-template-columns: repeat(auto-fit, minmax(220px,1fr)); gap:16px; margin-bottom:16px; }
    .kpi { padding:16px; }
    .num { font-size:32px; font-weight:700; }
    .tables { display:grid; grid-template-columns: repeat(auto-fit, minmax(320px,1fr)); gap:16px; }
    table { width:100%; }
    h3 { margin: 0 0 8px; }
  `]
})
export class DashboardComponent implements OnInit {
  private api = inject(StatsApi);

  clicks = signal(0);
  clients = signal(0);
  data = signal<ShortLinkEngagementSummaryResponse | null>(null);

  cols: string[] = ['key', 'value'];

  referrers = computed<KV[]>(() => {
    const m = this.data()?.referers || {};
    return Object.entries(m)
      .map(([key, value]) => ({ key, value }))
      .sort((a,b) => b.value - a.value)
      .slice(0, 10);
  });

  countries = computed<KV[]>(() => {
    const m = this.data()?.countries || {};
    return Object.entries(m)
      .map(([key, value]) => ({ key, value }))
      .sort((a,b) => b.value - a.value)
      .slice(0, 10);
  });

  ngOnInit() {
    this.api.summary().subscribe(s => {
      this.data.set(s);
      this.clicks.set(s.totalClicks ?? 0);
      this.clients.set(s.totalClients ?? 0);
    });
  }
}
