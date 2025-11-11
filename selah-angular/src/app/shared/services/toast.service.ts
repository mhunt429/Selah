import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'warning' | 'error';

export interface ToastMessage {
  message: string;
  type: ToastType;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private toastSubject = new BehaviorSubject<ToastMessage | null>(null);
  toast$ = this.toastSubject.asObservable();

  show(message: string, type: ToastType = 'success') {
    this.toastSubject.next({ message, type });
    setTimeout(() => this.dismiss(), 5000);
  }

  dismiss() {
    this.toastSubject.next(null);
  }
}
