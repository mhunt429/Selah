export interface Message {
  id: string;
  subject: string;
  body: string; // HTML content
  read: boolean;
  sentAt: string; // ISO date string
  sender?: string;
}
