import { NgClass } from '@angular/common';
import { Component, OnInit, signal, inject, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import {
  Settings,
  LogOut,
  Menu,
  X,
  Sun,
  Moon,
  Bell,
  LUCIDE_ICONS,
  LucideIconProvider,
  LucideAngularModule,
} from 'lucide-angular';
import { AuthService } from '../../services/auth.service';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  imports: [NgClass, LucideAngularModule],
  providers: [
    {
      provide: LUCIDE_ICONS,
      multi: true,
      useValue: new LucideIconProvider({
        Settings,
        LogOut,
        Menu,
        X,
        Sun,
        Moon,
        Bell,
      }),
    },
  ],
})
export class NavbarComponent implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  protected themeService = inject(ThemeService);

  @Input() sidebarOpen = false;
  @Output() sidebarToggle = new EventEmitter<void>();

  unreadNotifications = signal(3); // Mock unread count
  userName = signal('User');

  constructor() {
    // Load user info
    this.loadUserInfo();
  }

  ngOnInit() {
    // Load user info
    this.loadUserInfo();
  }

  private loadUserInfo() {
    const userJson = sessionStorage.getItem('appUser');
    if (userJson) {
      try {
        const user = JSON.parse(userJson);
        const fullName =
          user.firstName && user.lastName
            ? `${user.firstName} ${user.lastName}`
            : user.email || 'User';
        this.userName.set(fullName);
      } catch (e) {
        // If parsing fails, use default
      }
    }
  }

  toggleSidebar() {
    this.sidebarToggle.emit();
  }

  toggleDarkMode() {
    this.themeService.toggleDarkMode();
  }

  handleNotificationsClick() {
    // Mock: clear notifications
    this.unreadNotifications.set(0);
  }

  handleLogout() {
    // Clear session storage
    sessionStorage.clear();
    // Navigate to login
    this.router.navigate(['/identity/login']);
  }
}
