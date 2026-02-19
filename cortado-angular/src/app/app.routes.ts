import { Routes } from '@angular/router';
import { LandingPageComponent } from './features/landing-page/landing-page.component';
import { LoginComponent } from './features/identity/login/login.component';

import { DashboardComponent } from './features/dashboard/dashboard.component';
import { RegisterComponent } from './features/identity/register/register.component';
import { ConnectorComponent } from './features/connector/connector.component';
import { MainLayoutComponent } from './shared/components/main-layout/main-layout.component';
import { authGuard } from './shared/guards/auth-guard';
import { MessagesListComponent } from './features/messages/messages-list/messages-list.component';
import { MessageDetailComponent } from './features/messages/message-detail/message-detail.component';

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
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
      },
      { path: 'messages', component: MessagesListComponent },
      { path: 'messages/:id', component: MessageDetailComponent },
      { path: 'connector', component: ConnectorComponent },
    ],
  },
];
