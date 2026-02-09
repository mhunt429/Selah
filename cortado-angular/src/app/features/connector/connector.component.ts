import {
  Component,
  inject,
  Input,
  OnInit,
  OnChanges,
  SimpleChanges,
  OnDestroy,
} from '@angular/core';
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
export class ConnectorComponent implements OnInit, OnChanges, OnDestroy {
  constructor() {}
  @Input() linkToken = '';

  connectorService = inject(ConnectorService);
  router = inject(Router);
  private plaidHandler: any = null;

  ngOnInit() {
    if (this.linkToken) {
      this.initializePlaidLink();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['linkToken'] && this.linkToken && !changes['linkToken'].firstChange) {
      // Link token changed, reinitialize
      this.initializePlaidLink();
    }
  }

  ngOnDestroy() {
    // Clean up Plaid handler if it exists
    if (this.plaidHandler) {
      try {
        this.plaidHandler.destroy();
      } catch (e) {
        // Handler might already be destroyed
      }
      this.plaidHandler = null;
    }
  }

  private loadPlaidScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (document.getElementById('plaid-script') && (window as any).Plaid) {
        resolve();
        return;
      }

      if (document.getElementById('plaid-script')) {
        // Script tag exists but Plaid not loaded yet, wait for it
        const checkInterval = setInterval(() => {
          if ((window as any).Plaid) {
            clearInterval(checkInterval);
            resolve();
          }
        }, 100);

        // Timeout after 10 seconds
        setTimeout(() => {
          clearInterval(checkInterval);
          if ((window as any).Plaid) {
            resolve();
          } else {
            reject(new Error('Plaid script timeout'));
          }
        }, 10000);
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

  private initializePlaidLink() {
    // Destroy existing handler if it exists
    if (this.plaidHandler) {
      try {
        this.plaidHandler.destroy();
      } catch (e) {
        // Handler might already be destroyed
      }
      this.plaidHandler = null;
    }

    if (!this.linkToken) {
      console.error('Link token is required');
      return;
    }

    this.loadPlaidScript()
      .then(() => {
        // Small delay to ensure Plaid is fully ready
        setTimeout(() => {
          this.createAndOpenPlaidHandler();
        }, 100);
      })
      .catch((err) => console.error('Plaid script failed to load', err));
  }

  private createAndOpenPlaidHandler() {
    if (!(window as any).Plaid) {
      console.error('Plaid is not available');
      return;
    }

    try {
      this.plaidHandler = (window as any).Plaid.create({
        token: this.linkToken,
        onSuccess: (public_token: string, metadata: PlaidLinkMetadata) => {
          this.linkInstitution$(public_token, metadata).subscribe();
        },
        onExit: (err: any, metadata: any) => {
          console.log('Plaid exit:', err, metadata);
          // Clean up handler on exit
          if (this.plaidHandler) {
            try {
              this.plaidHandler.destroy();
            } catch (e) {
              // Handler might already be destroyed
            }
            this.plaidHandler = null;
          }
        },
      });

      if (this.plaidHandler) {
        this.plaidHandler.open();
      }
    } catch (error) {
      console.error('Error creating Plaid handler:', error);
    }
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
