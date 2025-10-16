import { Injectable } from '@angular/core';
import { Login } from '../../core/models/identity/login';
import { Observable } from 'rxjs';
import { AccessToken } from '../../core/models/identity/accessToken';
import { HttpClientService } from './http-client.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private httpClient: HttpClientService) {}
  public loginUser$(loginRequest: Login): Observable<AccessToken> {
    return this.httpClient.post$<AccessToken, Login>('/identity/login', loginRequest);
  }
}
