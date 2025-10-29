import { Injectable, inject } from '@angular/core';
import { HttpClientService } from './http-client.service';
import { Observable, of } from 'rxjs';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { ConnectorLinkTokenResponse } from '../../core/models/connector/connectorLinkTokenResponse';

@Injectable({
  providedIn: 'root',
})
export class ConnectorService {
  private httpClient = inject(HttpClientService);

  public getLinkToken$(): Observable<BaseApiResponse<ConnectorLinkTokenResponse>> {
    return this.httpClient.get$<BaseApiResponse<ConnectorLinkTokenResponse>>('connector/link');
  }
}
