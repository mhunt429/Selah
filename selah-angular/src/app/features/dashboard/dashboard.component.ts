import { Component, OnInit } from '@angular/core';
import { CardComponent } from '../../shared/card/card.component';
import { PagedTableComponent } from '../../shared/paged-table/paged-table.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  imports: [CardComponent, PagedTableComponent],
})
export class DashboardComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
