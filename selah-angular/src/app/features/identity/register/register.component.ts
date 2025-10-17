import { Component, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [FloatingLabelComponent, ReactiveFormsModule],
})
export class RegisterComponent implements OnInit {
  registrationForm = new FormGroup({
    accountName: new FormControl(''),
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    emailAddress: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    passwordConfirmation: new FormControl('', [Validators.required]),
  });

  constructor() {}

  ngOnInit() {}

  registrationSubmit(event: Event) {
    event.preventDefault();
  }
}
