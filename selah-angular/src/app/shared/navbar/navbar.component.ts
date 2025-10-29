import { NgClass } from '@angular/common';
import { Component, effect, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  imports: [RouterLink, NgClass],
})
export class NavbarComponent implements OnInit {
  mobileMenuOpen = false;

  selectedTab = signal('Dashboard');

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  constructor() {
    effect(() => console.log(this.selectedTab));
  }

  ngOnInit() {}

  setSelectedTab(tab: string) {
    this.selectedTab.set(tab);
  }
}
