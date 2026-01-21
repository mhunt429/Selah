import { NgClass } from '@angular/common';
import { Component, OnInit, signal, inject, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import {
  LayoutDashboard,
  CreditCard,
  Wallet,
  PiggyBank,
  LineChart,
  Menu,
  X,
  LUCIDE_ICONS,
  LucideIconProvider,
  LucideAngularModule,
} from 'lucide-angular';

interface NavigationItem {
  id: string;
  label: string;
  iconName: string;
  route: string;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
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
        Menu,
        X,
      }),
    },
  ],
})
export class SidebarComponent implements OnInit {
  private router = inject(Router);

  isOpen = signal(false);
  
  @Output() openChange = new EventEmitter<boolean>();
  
  // This will be set by the parent component
  @Input() set open(value: boolean) {
    this.isOpen.set(value);
  }
  currentPage = signal('dashboard');

  navigationItems: NavigationItem[] = [
    { id: 'dashboard', label: 'Dashboard', iconName: 'layout-dashboard', route: '/dashboard' },
    { id: 'accounts', label: 'Accounts', iconName: 'credit-card', route: '/accounts' },
    { id: 'transactions', label: 'Transactions', iconName: 'wallet', route: '/transactions' },
    { id: 'cash-flow', label: 'Cash Flow', iconName: 'piggy-bank', route: '/cash-flow' },
    { id: 'investments', label: 'Investments', iconName: 'line-chart', route: '/investments' },
  ];

  constructor() {}

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

  navigateToPage(item: NavigationItem) {
    this.currentPage.set(item.id);
    this.router.navigate([item.route]);
    // Close sidebar on mobile after navigation
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      this.isOpen.set(false);
      this.openChange.emit(false);
    }
  }

  isCurrentPage(itemId: string): boolean {
    return this.currentPage() === itemId;
  }

  closeSidebar() {
    this.isOpen.set(false);
    this.openChange.emit(false);
  }
}

