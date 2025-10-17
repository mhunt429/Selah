import { Component, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';
import { AuthService } from '../../../shared/services/auth.service';
import { Login } from '../../../core/models/identity/login';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [FloatingLabelComponent, ReactiveFormsModule],
})
export class LoginComponent implements OnInit {
  email: string = 'test';
  password: string = 'test';
  constructor(private authService: AuthService) {}

  ngOnInit() {}

  handleLogin(event: Event) {
    event.preventDefault();
    const request: Login = {
      email: this.email,
      password: this.password,
    };
    this.authService.loginUser$(request).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (e) => console.log(e),
    });
  }
}
