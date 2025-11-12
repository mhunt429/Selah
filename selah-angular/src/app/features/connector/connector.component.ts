import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-connector',
  templateUrl: './connector.component.html',
  styleUrls: ['./connector.component.scss'],
})
export class ConnectorComponent implements OnInit {
  constructor() {}
  @Input() linkToken = '';

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
      onSuccess: (public_token: string, metadata: any) => {
        console.log('Plaid success:', public_token, metadata);
      },
      onExit: (err: any, metadata: any) => {
        console.log('Plaid exit:', err, metadata);
      },
    });

    if (handler) handler.open();
  }
}
