import { Injectable, inject } from '@angular/core';
import { Observable, of, map } from 'rxjs';
import { HttpClientService } from './http-client.service';
import { BaseApiResponse } from '../../core/models/baseApiResponse';
import { Message } from '../../core/models/message/message';

// Mock data for development when messages API is not yet available
const MOCK_MESSAGES: Message[] = [
  {
    id: '1',
    subject: 'Welcome to Cortado',
    body: '<p>Hi there!</p><p>Welcome to <strong>Cortado.fi</strong>. We\'re glad to have you.</p><p>Get started by connecting your first account.</p>',
    read: false,
    sentAt: '2025-02-18T10:00:00Z',
    sender: 'Cortado Team',
  },
  {
    id: '2',
    subject: 'Your weekly summary',
    body: '<p>Here is your <em>weekly financial summary</em>.</p><ul><li>Net worth: up 2.3%</li><li>Savings rate: 15%</li></ul><p>View your <a href="/dashboard">dashboard</a> for details.</p>',
    read: true,
    sentAt: '2025-02-15T09:00:00Z',
    sender: 'Cortado',
  },
  {
    id: '3',
    subject: 'Security reminder',
    body: '<p>Please review your security settings.</p><p>We recommend enabling two-factor authentication.</p>',
    read: false,
    sentAt: '2025-02-10T14:30:00Z',
    sender: 'Cortado Security',
  },
];

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private httpClient = inject(HttpClientService);

  // In-memory store when using mock data (so markAsRead persists in session)
  private messagesStore = new Map<string, Message>(
    MOCK_MESSAGES.map((m) => [m.id, { ...m }])
  );

  /**
   * Fetch all messages for the current user.
   * Uses mock data; replace with API when available: this.httpClient.get$<BaseApiResponse<Message[]>>('/messages')
   */
  getMessages$(): Observable<Message[]> {
    const list = Array.from(this.messagesStore.values()).sort(
      (a, b) => new Date(b.sentAt).getTime() - new Date(a.sentAt).getTime()
    );
    return of(list);
  }

  /**
   * Fetch a single message by id.
   * Uses mock data; replace with: this.httpClient.get$<BaseApiResponse<Message>>(`/messages/${id}`)
   */
  getMessage$(id: string): Observable<Message | null> {
    const message = this.messagesStore.get(id) ?? null;
    return of(message);
  }

  /**
   * Mark a message as read.
   * Uses in-memory update for mock; replace with: this.httpClient.patch$<BaseApiResponse<void>>(`/messages/${id}/read`, {})
   */
  markAsRead$(id: string): Observable<void> {
    const message = this.messagesStore.get(id);
    if (message) {
      message.read = true;
      this.messagesStore.set(id, message);
    }
    return of(undefined);
  }
}
