import { Component, inject, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordMatchValidator } from '../../../core/validators';
import { PasswordValidationSummaryComponent } from '../../../shared/password-validation-summary/password-validation-summary.component';
import { AccountService } from '../../../shared/services/account.service';
import { UserRegistration } from '../../../core/models/identity/userRegistration';
import { AlertComponent, AlertType } from '../../../shared/alert/alert.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [
    FloatingLabelComponent,
    ReactiveFormsModule,
    PasswordValidationSummaryComponent,
    AlertComponent,
  ],
})
export class RegisterComponent implements OnInit {
  AlertType = AlertType;
  private accountService = inject(AccountService);

  errors: string[] = [];

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

    const request: UserRegistration = {
      accountName: this.registrationForm.get('accountName')?.value || '',
      firstName: this.registrationForm.get('firstName')?.value || '',
      lastName: this.registrationForm.get('lastName')?.value || '',
      email: this.registrationForm.get('emailAddress')?.value || '',
      password: this.registrationForm.get('password')?.value || '',
      passwordConfirmation: this.registrationForm.get('passwordConfirmation')?.value || '',
    };

    this.accountService.registerAccount$(request).subscribe({
      next: (v) => {
        console.log(v);
      },
      error: (e) => (this.errors = e.error.errors),
    });
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
