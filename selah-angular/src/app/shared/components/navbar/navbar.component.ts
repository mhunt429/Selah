import { NgClass } from '@angular/common';
import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  LayoutDashboard,
  CreditCard,
  Wallet,
  PiggyBank,
  LineChart,
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

interface NavigationItem {
  id: string;
  label: string;
  iconName: string;
  route: string;
}

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
        LayoutDashboard,
        CreditCard,
        Wallet,
        PiggyBank,
        LineChart,
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

  mobileMenuOpen = false;
  unreadNotifications = signal(3); // Mock unread count
  currentPage = signal('dashboard');
  userName = signal('User');

  navigationItems: NavigationItem[] = [
    { id: 'dashboard', label: 'Dashboard', iconName: 'layout-dashboard', route: '/dashboard' },
    { id: 'accounts', label: 'Accounts', iconName: 'credit-card', route: '/accounts' },
    { id: 'transactions', label: 'Transactions', iconName: 'wallet', route: '/transactions' },
    { id: 'cash-flow', label: 'Cash Flow', iconName: 'piggy-bank', route: '/cash-flow' },
    { id: 'investments', label: 'Investments', iconName: 'line-chart', route: '/investments' },
  ];

  constructor() {
    // Load user info
    this.loadUserInfo();
  }

  ngOnInit() {
    // Set current page based on route
    this.updateCurrentPageFromRoute();
    this.router.events.subscribe(() => {
      this.updateCurrentPageFromRoute();
    });
  }

  private updateCurrentPageFromRoute() {
    const currentRoute = this.router.url.split('?')[0];
    const routeParts = currentRoute.split('/').filter((part) => part);
    const page = routeParts[routeParts.length - 1] || 'dashboard';
    this.currentPage.set(page);
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

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
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

  navigateToPage(item: NavigationItem) {
    this.currentPage.set(item.id);
    this.router.navigate([item.route]);
    if (this.mobileMenuOpen) {
      this.mobileMenuOpen = false;
    }
  }

  isCurrentPage(itemId: string): boolean {
    return this.currentPage() === itemId;
  }
}
