import { Component, OnInit, inject } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';
import { AuthService } from '../../../shared/services/auth.service';
import { Login } from '../../../core/models/identity/login';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [FloatingLabelComponent, ReactiveFormsModule],
})
export class LoginComponent implements OnInit {
  form!: FormGroup;
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  ngOnInit() {
    this.form = this.fb.group({
      email: [''],
      password: [''],
    });
  }
  constructor() {}

  handleLogin(event: Event) {
    event.preventDefault();
    const request: Login = this.form.value;
    this.authService.loginUser$(request).subscribe({
      next: (response) => console.log(response),
      error: (e) => console.log(e),
    });
  }
}
