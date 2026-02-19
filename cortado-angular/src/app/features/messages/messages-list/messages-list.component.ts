import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgClass, DatePipe } from '@angular/common';
import { CardComponent } from '../../../shared/components/card/card.component';
import { MessagesService } from '../../../shared/services/messages.service';
import { Message } from '../../../core/models/message/message';
import { LUCIDE_ICONS, LucideIconProvider, Mail, LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-messages-list',
  templateUrl: './messages-list.component.html',
  styleUrls: ['./messages-list.component.scss'],
  imports: [RouterLink, NgClass, DatePipe, CardComponent, LucideAngularModule],
  providers: [
    {
      provide: LUCIDE_ICONS,
      multi: true,
      useValue: new LucideIconProvider({ Mail }),
    },
  ],
})
export class MessagesListComponent implements OnInit {
  private messagesService = inject(MessagesService);

  messages: Message[] = [];
  loading = true;

  ngOnInit() {
    this.messagesService.getMessages$().subscribe({
      next: (list) => {
        this.messages = list;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
