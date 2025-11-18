import { Component, OnInit, inject } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/components/inputs/floating-label/floating-label.component';
import { AuthService } from '../../../shared/services/auth.service';
import { Login } from '../../../core/models/identity/login';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AlertComponent, AlertType } from '../../../shared/components/alert/alert.component';
import { PrimaryButtonComponent } from '../../../shared/components/primary-button/primary-button.component';
import { passwordMatchValidator } from '../../../core/validators';
import { BaseApiResponse } from '../../../core/models/baseApiResponse';
import { AccessToken } from '../../../core/models/identity/accessToken';
import { switchMap, tap } from 'rxjs';
import { AppUser } from '../../../core/models/appUser/appUser';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [FloatingLabelComponent, ReactiveFormsModule, AlertComponent, PrimaryButtonComponent],
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  AlertType = AlertType;

  loginError = '';

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl('', [Validators.required]),
    rememberMe: new FormControl(false),
  });

  ngOnInit() {}
  constructor(private router: Router) {}

  handleLogin(event: Event) {
    event.preventDefault();

    const request: Login = {
      email: this.loginForm.get('email')?.value || '',
      password: this.loginForm.get('password')?.value || '',
      rememberMe: this.loginForm.get('rememberMe')?.value || false,
    };
    this.authService
      .loginUser$(request)
      .pipe(
        tap((loginRsp: BaseApiResponse<AccessToken>) => {
          const tokenData = loginRsp.data;
          sessionStorage.setItem('accessToken', tokenData.accessToken);
          sessionStorage.setItem('sessionExpiration', tokenData.accessTokenExpiration);
        }),
        switchMap(() => this.authService.loadSession$())
      )
      .subscribe({
        next: (userRsp: BaseApiResponse<AppUser>) => {
          this.router.navigateByUrl('/dashboard');
        },
        error: (e) => {
          console.error(e);
          if (e.status === 401) {
            this.loginError = 'Please check your login credentials and try again.';
          } else {
            this.loginError = `We're unable to sign you in at this moment. Please try again later.`;
          }
        },
      });
  }
}
