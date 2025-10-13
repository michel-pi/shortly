import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of, switchMap, tap } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, UserResponse } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private _user = new BehaviorSubject<UserResponse | null>(null);
  user$ = this._user.asObservable();

  private accessTokenKey = 'shortly.accessToken';

  get token(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  set token(v: string | null) {
    if (v) {
      localStorage.setItem(this.accessTokenKey, v);
    } else {
      localStorage.removeItem(this.accessTokenKey);
    }
  }

  me(): Observable<UserResponse> {
    return this.http
      .get<UserResponse>(`${environment.apiBaseUrl}/Auth/user`)
      .pipe(tap(u => this._user.next(u)));
  }

  login(body: LoginRequest): Observable<void> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/Auth/login`, body, { withCredentials: true })
      .pipe(
        tap(res => {
          if (res.accessToken) this.token = res.accessToken;
        }),
        switchMap(() => this.me()),
        switchMap(() => of(void 0))
      );
  }

  refresh(): Observable<string | null> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/refresh`, {}, { withCredentials: true })
      .pipe(
        tap(res => {
          if (res.accessToken) this.token = res.accessToken;
        }),
        switchMap(res => of(res.accessToken ?? null))
      );
  }

  logout(): Observable<void> {
    const t = this.token;
    const headers = t ? new HttpHeaders({ Authorization: `Bearer ${t}` }) : undefined;
    const skipRefresh = new HttpHeaders({ 'x-skip-refresh': '1' });

    return this.http
      .post<void>(
        `${environment.apiBaseUrl}/auth/logout`,
        {},
        { withCredentials: true, headers: headers?.append('x-skip-refresh', '1') ?? skipRefresh }
      )
      .pipe(
        finalize(() => {
          this.token = null;
          this._user.next(null);
        })
      );
  }
}
