import { inject, Injectable } from '@angular/core';
import { Login } from '../../core/models/identity/login';
import { Observable } from 'rxjs';
import { AccessToken } from '../../core/models/identity/accessToken';
import { HttpClientService } from './http-client.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { getCookieValue } from '../helpers/cookie-helper';

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

  public getAccessToken() {
    return sessionStorage.getItem('access_token');
  }

  public getSessionId() {
    return sessionStorage.getItem('sessionId');
  }

  public isAuthenticated(): boolean {
    const x_api_token = getCookieValue('x_api_token');
    const sessionExpiration = parseInt(sessionStorage.getItem('sessionExpiration') ?? '18000');
    return x_api_token !== '' && Date.now() < sessionExpiration;
  }
}
