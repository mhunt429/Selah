import { Component, inject, OnInit } from '@angular/core';
import { CardComponent } from '../../shared/components/card/card.component';
import {
  ColumnHeader,
  PagedTableComponent,
} from '../../shared/components/paged-table/paged-table.component';
import { LUCIDE_ICONS, LucideIconProvider, Plus, LucideAngularModule } from 'lucide-angular';
import { PrimaryButtonComponent } from '../../shared/components/primary-button/primary-button.component';
import { ConnectorService } from '../../shared/services/connector.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { ConnectorLinkTokenResponse } from '../../core/models/connector/connectorLinkTokenResponse';
import { ConnectorComponent } from '../connector/connector.component';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  imports: [
    CardComponent,
    PagedTableComponent,
    LucideAngularModule,
    PrimaryButtonComponent,
    ConnectorComponent,
  ],
  providers: [
    {
      provide: LUCIDE_ICONS,
      multi: true,
      useValue: new LucideIconProvider({
        Plus,
      }),
    },
  ],
})
export class DashboardComponent implements OnInit {
  private connectorService = inject(ConnectorService);
  private toastService = inject(ToastService);
  showEmptyState: boolean = true;

  showConnectorOverlay = false;
  linkToken = '';

  recentTransactionTableHeaders: ColumnHeader[] = [
    { name: 'Date', sortable: true },
    {
      name: 'Location',
      sortable: true,
      //handleSort$: undefined
    },
    {
      name: 'Amount',
      sortable: true,
      //handleSort$: undefined
    },
    { name: 'Category', sortable: true },
    { name: 'Pending', sortable: false },
  ];
  constructor() {}

  ngOnInit() {}

  linkAccount(e: Event) {
    e.preventDefault;
    this.connectorService.getLinkToken$().subscribe({
      next: (result: BaseApiResponse<ConnectorLinkTokenResponse>) => {
        if (result.errors) {
          this.toastService.show('Unable to link institution.', 'error');
        } else {
          this.linkToken = result.data.link_token;
          this.showConnectorOverlay = true;
        }
      },
    });
  }
}
