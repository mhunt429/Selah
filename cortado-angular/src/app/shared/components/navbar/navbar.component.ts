import { NgClass } from '@angular/common';
import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
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
  ChevronDown,
  Repeat,
  Mail,
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
  hasDropdown?: boolean;
  dropdownItems?: DropdownItem[];
}

interface DropdownItem {
  id: string;
  label: string;
  route: string;
  iconName?: string;
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
        ChevronDown,
        Repeat,
        Mail,
      }),
    },
  ],
})
export class NavbarComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private authService = inject(AuthService);
  protected themeService = inject(ThemeService);

  mobileMenuOpen = false;
  transactionsDropdownOpen = signal(false);
  mobileTransactionsDropdownOpen = signal(false);
  unreadNotifications = signal(3); // Mock unread count
  currentPage = signal('dashboard');
  userName = signal('User');
  private clickOutsideHandler = this.handleClickOutside.bind(this);

  navigationItems: NavigationItem[] = [
    { id: 'dashboard', label: 'Dashboard', iconName: 'layout-dashboard', route: '/dashboard' },
    { id: 'accounts', label: 'Accounts', iconName: 'credit-card', route: '/accounts' },
    {
      id: 'transactions',
      label: 'Transactions',
      iconName: 'wallet',
      route: '/transactions',
      hasDropdown: true,
      dropdownItems: [
        {
          id: 'transactions',
          label: 'All Transactions',
          route: '/transactions',
          iconName: 'wallet',
        },
        {
          id: 'recurring-transactions',
          label: 'Recurring Transactions',
          route: '/transactions/recurring',
          iconName: 'repeat',
        },
      ],
    },
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

    // Close dropdown when clicking outside
    if (typeof document !== 'undefined') {
      document.addEventListener('click', this.clickOutsideHandler);
    }
  }

  private handleClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    // Check if click is outside the dropdown
    if (!target.closest('.transactions-dropdown-container')) {
      this.transactionsDropdownOpen.set(false);
    }
  }

  ngOnDestroy() {
    // Clean up event listener
    if (typeof document !== 'undefined') {
      document.removeEventListener('click', this.clickOutsideHandler);
    }
  }

  private updateCurrentPageFromRoute() {
    const currentRoute = this.router.url.split('?')[0];
    const routeParts = currentRoute.split('/').filter((part) => part);
    // Keep 'messages' active when viewing message list or message detail
    const page =
      routeParts[0] === 'messages' ? 'messages' : routeParts[routeParts.length - 1] || 'dashboard';
    this.currentPage.set(page);

    // Close dropdowns when route changes
    this.transactionsDropdownOpen.set(false);
    this.mobileTransactionsDropdownOpen.set(false);
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
    this.router.navigate(['/messages']);
  }

  handleLogout() {
    // Clear session storage
    sessionStorage.clear();
    // Navigate to login
    this.router.navigate(['/identity/login']);
  }

  navigateToPage(item: NavigationItem) {
    if (item.hasDropdown) {
      // Toggle dropdown instead of navigating
      this.transactionsDropdownOpen.set(!this.transactionsDropdownOpen());
      return;
    }

    this.currentPage.set(item.id);
    this.router.navigate([item.route]);
    if (this.mobileMenuOpen) {
      this.mobileMenuOpen = false;
    }
  }

  navigateToDropdownItem(item: NavigationItem, dropdownItem: DropdownItem) {
    this.currentPage.set(dropdownItem.id);
    this.router.navigate([dropdownItem.route]);
    this.transactionsDropdownOpen.set(false);
    this.mobileTransactionsDropdownOpen.set(false);
    if (this.mobileMenuOpen) {
      this.mobileMenuOpen = false;
    }
  }

  toggleTransactionsDropdown() {
    this.transactionsDropdownOpen.set(!this.transactionsDropdownOpen());
  }

  toggleMobileTransactionsDropdown() {
    this.mobileTransactionsDropdownOpen.set(!this.mobileTransactionsDropdownOpen());
  }

  isCurrentPage(itemId: string): boolean {
    return this.currentPage() === itemId;
  }

  isCurrentDropdownPage(itemId: string): boolean {
    return this.currentPage() === itemId;
  }
}
