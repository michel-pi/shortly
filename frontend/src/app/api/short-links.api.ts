import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ShortLinkDto {
  id: number;
  shortCode: string;
  targetUrl: string;
  isActive: boolean;
  createdAt?: string;
  changedAt?: string;
}

export interface CreateShortLinkDto {
  targetUrl: string;
  isActive: boolean;
}

export interface UpdateShortLinkDto {
  targetUrl?: string;
  isActive?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ShortLinksApi {
  private http = inject(HttpClient);
  private base = `${environment.apiBaseUrl}/ShortLinks`;

  list(skip?: number, take?: number): Observable<ShortLinkDto[]> {
    let params = new HttpParams();
    if (skip !== undefined && skip !== null) params = params.set('skip', skip);
    if (take !== undefined && take !== null) params = params.set('take', take);
    return this.http.get<ShortLinkDto[]>(this.base, { params });
  }

  get(id: number): Observable<ShortLinkDto> {
    return this.http.get<ShortLinkDto>(`${this.base}/${id}`);
  }

  create(body: CreateShortLinkDto): Observable<ShortLinkDto> {
    return this.http.post<ShortLinkDto>(this.base, body);
  }

  update(id: number, body: UpdateShortLinkDto): Observable<ShortLinkDto> {
    return this.http.put<ShortLinkDto>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
