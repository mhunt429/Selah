import { Component } from '@angular/core';
import { ToastService } from '../../services/toast.service';
import { AsyncPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  imports: [NgClass, AsyncPipe],
})
export class ToastComponent {
  constructor(public toastService: ToastService) {}
}
