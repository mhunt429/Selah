import { Component, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordMatchValidator } from '../../../core/validators';
import { PasswordValidationSummaryComponent } from '../../../shared/password-validation-summary/password-validation-summary.component';
import { PagedTableComponent } from '../../../shared/paged-table/paged-table.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [FloatingLabelComponent, ReactiveFormsModule, PasswordValidationSummaryComponent],
})
export class RegisterComponent implements OnInit {
  registrationForm = new FormGroup(
    {
      accountName: new FormControl(''),
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      emailAddress: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      passwordConfirmation: new FormControl('', [Validators.required]),
    },
    { validators: passwordMatchValidator }
  );

  passwordRules = {
    minLength: false,
    number: false,
    symbol: false,
    uppercase: false,
    lowercase: false,
  };

  constructor() {}

  ngOnInit() {}

  registrationSubmit(event: Event) {
    event.preventDefault();
  }

  updatePasswordRules() {
    const value = this.registrationForm.get('password')?.value || '';

    this.passwordRules.minLength = value.length >= 8;
    this.passwordRules.number = /\d/.test(value);
    this.passwordRules.symbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    this.passwordRules.uppercase = /[A-Z]/.test(value);
    this.passwordRules.lowercase = /[a-z]/.test(value);
  }
}
