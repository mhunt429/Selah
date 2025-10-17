import { Component, OnInit } from '@angular/core';
import { CardComponent } from '../../shared/card/card.component';
import { ColumnHeader, PagedTableComponent } from '../../shared/paged-table/paged-table.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  imports: [CardComponent, PagedTableComponent],
})
export class DashboardComponent implements OnInit {
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
}
