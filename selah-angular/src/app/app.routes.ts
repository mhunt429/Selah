import { Routes } from '@angular/router';
import { LandingPageComponent } from './features/landing-page/landing-page.component';
import { LoginComponent } from './features/identity/login/login.component';
import { MainLayoutComponent } from './shared/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { RegisterComponent } from './features/identity/register/register.component';
import { ConnectorComponent } from './features/connector/connector/connector.component';

export const routes: Routes = [
  {
    path: '',
    component: LandingPageComponent,
  },
  {
    path: 'identity',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
    ],
  },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
      },
      { path: 'connector', component: ConnectorComponent },
    ],
  },
];
