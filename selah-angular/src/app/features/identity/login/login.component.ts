import { Component, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [FloatingLabelComponent],
})
export class LoginComponent implements OnInit {
  // private titleService: TitleSer;
  constructor() {}

  ngOnInit() {}
}
