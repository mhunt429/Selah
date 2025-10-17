import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-floating-label',
  templateUrl: './floating-label.component.html',
  styleUrls: ['./floating-label.component.scss'],
  imports: [ReactiveFormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FloatingLabelComponent),
      multi: true,
    },
  ],
})
export class FloatingLabelComponent implements OnInit {
  @Input() id: string = '';
  @Input() label: string = '';
  @Input() name: string = '';
  @Input() type: string = '';
  @Input() required: boolean = false;

  value?: number | string | Date = undefined;

  constructor() {}

  ngOnInit() {}

  private onChange: any = () => {};
  private onTouched: any = () => {};

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.value = input.value;
    this.onChange(this.value);
  }
}
