import { Component } from '@angular/core';
import { ToastService, ToastType } from '../../services/toast.service';
import { AsyncPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss'],
  imports: [NgClass, AsyncPipe],
})
export class ToastComponent {
  ToastType = ToastType;

  constructor(public toastService: ToastService) {}
}
