import { Component, OnInit, Input } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss'],
  imports: [NgClass],
})
export class CardComponent implements OnInit {
  @Input() cardClasses = 'rounded-lg shadow p-6';

  constructor() {}

  ngOnInit() {}
}
