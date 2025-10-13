import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AccessKeyDto {
  id: number;
  token: string;
  name?: string;
  isActive: boolean;
  createdAt?: string;
  changedAt?: string;
}

export interface CreateAccessKeyDto {
  name?: string;
  isActive: boolean;
}

export interface UpdateAccessKeyDto {
  name?: string;
  isActive?: boolean;
}

@Injectable({ providedIn: 'root' })
export class AccessKeysApi {
  private http = inject(HttpClient);
  private base = `${environment.apiBaseUrl}/AccessKeys`;

  list(skip?: number, take?: number): Observable<AccessKeyDto[]> {
    let params = new HttpParams();
    if (skip !== undefined && skip !== null) params = params.set('skip', skip);
    if (take !== undefined && take !== null) params = params.set('take', take);
    return this.http.get<AccessKeyDto[]>(this.base, { params });
  }

  get(id: number): Observable<AccessKeyDto> {
    return this.http.get<AccessKeyDto>(`${this.base}/${id}`);
  }

  create(body: CreateAccessKeyDto): Observable<AccessKeyDto> {
    return this.http.post<AccessKeyDto>(this.base, body);
  }

  update(id: number, body: UpdateAccessKeyDto): Observable<AccessKeyDto> {
    return this.http.put<AccessKeyDto>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
