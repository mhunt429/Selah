import { Injectable, inject } from '@angular/core';
import { HttpClientService } from './http-client.service';
import { Observable, of } from 'rxjs';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { ConnectorLinkTokenResponse } from '../../core/models/connector/connectorLinkTokenResponse';
import { PlaidLinkTokenRequest } from '../../core/models/connector/plaidLinkTokenRequest';

@Injectable({
  providedIn: 'root',
})
export class ConnectorService {
  private httpClient = inject(HttpClientService);

  public getLinkToken$(): Observable<BaseApiResponse<ConnectorLinkTokenResponse>> {
    return this.httpClient.get$<BaseApiResponse<ConnectorLinkTokenResponse>>('/connector/link');
  }

  public exchangeToken$(request: PlaidLinkTokenRequest): Observable<void> {
    return this.httpClient.post$('/connector/exchange', request);
  }
}
