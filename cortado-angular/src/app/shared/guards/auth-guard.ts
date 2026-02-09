import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { AppUser } from '../../core/models/appUser/appUser';
import { map, catchError, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.loadSession$().pipe(
    map((rsp: BaseApiResponse<AppUser>) => {
      sessionStorage.setItem('appUser', JSON.stringify(rsp));

      return true;
    }),
    catchError((err) => {
      // session load failed, redirect to login
      router.navigate(['/identity/login']);
      return of(false);
    })
  );
};
