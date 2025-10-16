import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientService } from './shared/services/http-client.service';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  providers: [HttpClientService, AuthService],
})
export class App {
  protected readonly title = signal('Selah.fi');
}
