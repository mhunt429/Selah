import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientService } from './shared/services/http-client.service';
import { AuthService } from './shared/services/auth.service';
import { LucideAngularModule, FileIcon } from 'lucide-angular';
import { ToastComponent } from './shared/components/toast/toast.component';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LucideAngularModule, ToastComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  providers: [HttpClientService, AuthService],
})
export class App {
  protected readonly title = signal('Selah.fi');
}
