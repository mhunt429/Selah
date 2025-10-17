import { Component, OnInit } from '@angular/core';
import { FloatingLabelComponent } from '../../../shared/inputs/floating-label/floating-label.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [FloatingLabelComponent],
})
export class RegisterComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
