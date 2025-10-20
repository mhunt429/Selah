import { inject, Injectable } from '@angular/core';
import { Login } from '../../core/models/identity/login';
import { Observable } from 'rxjs';
import { AccessToken } from '../../core/models/identity/accessToken';
import { HttpClientService } from './http-client.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';

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
}
