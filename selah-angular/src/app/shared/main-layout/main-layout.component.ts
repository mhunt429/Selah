import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css'],
  imports: [RouterOutlet, NavbarComponent],
})
export class MainLayoutComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
