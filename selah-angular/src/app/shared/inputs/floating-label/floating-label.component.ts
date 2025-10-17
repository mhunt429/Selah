import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-floating-label',
  templateUrl: './floating-label.component.html',
  styleUrls: ['./floating-label.component.scss'],
})
export class FloatingLabelComponent implements OnInit {
  @Input() id: string = '';
  @Input() label: string = '';
  @Input() name: string = '';
  @Input() type: string = '';
  @Input() required: boolean = false;

  constructor() {}

  ngOnInit() {}
}
