import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export enum ToastType {
  Success,
  Error,
  Warning,
  Info,
}

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

  show(message: string, type: ToastType = ToastType.Success, persisted: boolean = false) {
    this.toastSubject.next({ message, type });
    if (!persisted) {
      setTimeout(() => this.dismiss(), 5000);
    }
  }

  dismiss() {
    this.toastSubject.next(null);
  }
}
