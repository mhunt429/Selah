import { inject, Injectable } from '@angular/core';
import { Login } from '../../core/models/identity/login';
import { Observable } from 'rxjs';
import { AccessToken } from '../../core/models/identity/accessToken';
import { HttpClientService } from './http-client.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { getCookieValue } from '../helpers/cookie-helper';
import { AppUser } from '../../core/models/appUser/appUser';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient = inject(HttpClientService);
  constructor() {}
  public loginUser$(loginRequest: Login): Observable<BaseApiResponse<AccessToken>> {
    return this.httpClient.post$<BaseApiResponse<AccessToken>, Login>(
      '/identity/login',
      loginRequest
    );
  }

  public loadSession$(): Observable<BaseApiResponse<AppUser>> {
    return this.httpClient.get$<BaseApiResponse<AppUser>>('/identity/current-user');
  }

  public getSessionId() {
    return sessionStorage.getItem('sessionId');
  }

  public isAuthenticated(): boolean {
    const user = sessionStorage.getItem('appUser');
    const accessTokenExpiration = parseInt(
      sessionStorage.getItem('accessTokenExpiration') ?? '18000'
    );
    return user !== '' && Date.now() < accessTokenExpiration;
  }
}
