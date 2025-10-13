import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

let isRefreshing = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token;
  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(authReq).pipe(
    catchError((err: any) => {
        const is401 = err?.status === 401;
        const isAuthEndpoint = /\/api\/auth\/(login|refresh|logout)/i.test(req.url);
        const skip = req.headers.has('x-skip-refresh');
        if (is401 && !isAuthEndpoint && !isRefreshing && !skip) {
        isRefreshing = true;
        return auth.refresh().pipe(
          switchMap(newToken => {
            isRefreshing = false;
            if (newToken) {
              const retry = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
              return next(retry);
            }
            auth.token = null;
            return throwError(() => err);
          }),
          catchError(e => { isRefreshing = false; auth.token = null; return throwError(() => e); })
        );
      }
      return throwError(() => err);
    })
  );
};
