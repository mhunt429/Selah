import { Component, Input, OnInit } from '@angular/core';
import {
  ArrowLeft,
  ArrowRight,
  ArrowUpDown,
  LUCIDE_ICONS,
  LucideAngularModule,
  LucideIconProvider,
} from 'lucide-angular';

export type ColumnHeader = {
  name: string;
  sortable: boolean;
  handleSort$: (name: string) => void;
};

@Component({
  selector: 'app-paged-table',
  templateUrl: './paged-table.component.html',
  styleUrls: ['./paged-table.component.scss'],
  imports: [LucideAngularModule],
  providers: [
    {
      provide: LUCIDE_ICONS,
      multi: true,
      useValue: new LucideIconProvider({
        ArrowLeft,
        ArrowRight,
        ArrowUpDown,
      }),
    },
  ],
})
export class PagedTableComponent implements OnInit {
  @Input() columnsHeaders: ColumnHeader[] = [];
  @Input() totalPages: number = 0;

  currentPage: number = 1;

  constructor() {}

  ngOnInit() {}

  get pages() {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}
