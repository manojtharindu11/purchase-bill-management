import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // A failed login is an expected validation outcome and must surface to the
      // user - only treat 401s on *other* endpoints as an expired session.
      if (error.status === 401 && !req.url.includes('/auth/login')) {
        auth.logout();
        void router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
