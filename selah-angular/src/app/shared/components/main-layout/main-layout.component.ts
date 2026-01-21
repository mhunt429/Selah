import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  imports: [RouterOutlet, NavbarComponent, SidebarComponent, NgClass],
})
export class MainLayoutComponent implements OnInit {
  sidebarOpen = signal(true); // Start with sidebar open by default

  constructor() {}

  ngOnInit() {
    // On mobile, start with sidebar closed
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      this.sidebarOpen.set(false);
    }
  }

  toggleSidebar() {
    this.sidebarOpen.set(!this.sidebarOpen());
  }
}
