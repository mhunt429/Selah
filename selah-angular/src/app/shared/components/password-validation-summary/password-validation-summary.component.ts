import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-password-validation-summary',
  templateUrl: './password-validation-summary.component.html',
  styleUrls: ['./password-validation-summary.component.scss'],
})
export class PasswordValidationSummaryComponent implements OnInit {
  @Input() greaterThanOrEqualToMinLength: boolean = false;
  @Input() containsNumber: boolean = false;
  @Input() containsSymbol: boolean = false;
  @Input() containsUpperCase: boolean = false;
  @Input() containsLowerCase: boolean = false;

  constructor() {}

  ngOnInit() {}
}
