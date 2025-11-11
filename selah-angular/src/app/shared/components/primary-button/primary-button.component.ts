import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-primary-button',
  templateUrl: './primary-button.component.html',
  styleUrls: ['./primary-button.component.scss'],
})
export class PrimaryButtonComponent implements OnInit {
  @Output() onClick = new EventEmitter<Event>();

  constructor() {}

  ngOnInit() {}

  onClickButton(event: Event) {
    this.onClick.emit(event);
  }
}
