import { Component, signal, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientService } from './shared/services/http-client.service';
import { AuthService } from './shared/services/auth.service';
import { ThemeService } from './shared/services/theme.service';
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
  protected readonly title = signal('Cortado.fi');

  // Inject ThemeService to ensure it's initialized early (constructor runs immediately)
  // Since ThemeService is providedIn: 'root', we don't need to add it to providers
  constructor(private themeService: ThemeService) {
    // Service will initialize and apply dark mode on construction
  }
}
