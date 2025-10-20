import { Component, Input, OnInit } from '@angular/core';

export enum AlertType {
  Success,
  Info,
  Warning,
  Error,
}

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss'],
})
export class AlertComponent implements OnInit {
  @Input() alertType: AlertType = AlertType.Success;
  AlertType = AlertType;

  isVisible = true;

  constructor() {}

  ngOnInit() {}

  closeAlert() {
    this.isVisible = false;
  }
}
