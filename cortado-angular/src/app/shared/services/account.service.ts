import { inject, Injectable } from '@angular/core';
import { HttpClientService } from './http-client.service';
import { UserRegistration } from '../../core/models/identity/userRegistration';
import { Observable } from 'rxjs';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { AccessToken } from '../../core/models/identity/accessToken';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private httpClient = inject(HttpClientService);
  constructor() {}

  public registerAccount$(
    userRegistration: UserRegistration
  ): Observable<BaseApiResponse<AccessToken>> {
    return this.httpClient.post$<BaseApiResponse<AccessToken>, UserRegistration>(
      '/account/register',
      userRegistration
    );
  }
}
