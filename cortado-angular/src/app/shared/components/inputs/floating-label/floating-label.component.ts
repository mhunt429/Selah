import { Component, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-floating-label',
  standalone: true,
  templateUrl: './floating-label.component.html',
  styleUrls: ['./floating-label.component.scss'],
  imports: [FormsModule, ReactiveFormsModule, NgClass],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FloatingLabelComponent),
      multi: true,
    },
  ],
})
export class FloatingLabelComponent {
  @Input() id = '';
  @Input() label = '';
  @Input() name = '';
  @Input() type = 'text';
  @Input() required = false;
  @Input() classes = '';

  value: string | number | Date | null = '';

  onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: any): void {
    this.value = value ?? '';
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

  onBlur() {
    this.onTouched();
  }
}
