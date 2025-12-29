import { Component, inject, Input, OnInit } from '@angular/core';
import { PlaidLinkMetadata } from '../../core/models/connector/plaidLinkMetadata';
import { Observable, of, tap } from 'rxjs';
import { ConnectorService } from '../../shared/services/connector.service';
import { PlaidLinkTokenRequest } from '../../core/models/connector/plaidLinkTokenRequest';
import { Router } from '@angular/router';

@Component({
  selector: 'app-connector',
  templateUrl: './connector.component.html',
  styleUrls: ['./connector.component.scss'],
})
export class ConnectorComponent implements OnInit {
  constructor() {}
  @Input() linkToken = '';

  connectorService = inject(ConnectorService);
  router = inject(Router);

  ngOnInit() {
    this.loadPlaidScript()
      .then(() => this.initializePlaid())
      .catch((err) => console.error('Plaid script failed to load', err));
  }

  private loadPlaidScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (document.getElementById('plaid-script')) {
        resolve();
        return;
      }

      const script = document.createElement('script');
      script.id = 'plaid-script';
      script.src = 'https://cdn.plaid.com/link/v2/stable/link-initialize.js';
      script.async = true;
      script.onload = () => resolve();
      script.onerror = (err) => reject(err);

      document.body.appendChild(script);
    });
  }
  private initializePlaid() {
    const handler = (window as any).Plaid?.create({
      token: this.linkToken,
      onSuccess: (public_token: string, metadata: PlaidLinkMetadata) => {
        this.linkInstitution$(public_token, metadata).subscribe();
      },
      onExit: (err: any, metadata: any) => {
        console.log('Plaid exit:', err, metadata);
      },
    });

    if (handler) handler.open();
  }

  private linkInstitution$(publicToken: string, metadata: PlaidLinkMetadata): Observable<void> {
    const request: PlaidLinkTokenRequest = {
      publicToken,
      institutionId: metadata.institution.institution_id,
      institutionName: metadata.institution.name,
    };

    return this.connectorService.exchangeToken$(request).pipe(
      tap({
        next: () => {
          this.router.navigateByUrl('/dashboard');
        },
      })
    );
  }
}
