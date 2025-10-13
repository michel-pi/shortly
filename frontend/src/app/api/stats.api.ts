import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ShortLinkEngagementSummaryResponse {
  totalClicks: number;
  totalClients: number;
  countries?: Record<string, number>;
  referers?: Record<string, number>;
  from?: string;
  to?: string;
}

@Injectable({ providedIn: 'root' })
export class StatsApi {
  private http = inject(HttpClient);

  summary(params?: { includeInactive?: boolean; from?: string; to?: string }) {
    return this.http.get<ShortLinkEngagementSummaryResponse>(
      `${environment.apiBaseUrl}/ShortLinkEngagements/summary`,
      { params: (params as any) || {} }
    );
  }
}
