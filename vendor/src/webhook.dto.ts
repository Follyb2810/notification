export class TextWebhookDto {
  reference?: string;
  phoneNumber?: string;
  message?: string;
  senderId?: string;
}

export class EmailWebhookDto {
  reference?: string;
  to?: string;
  subject?: string;
  body?: string;
  isHtml?: boolean;
}

export class DeliveryReceiptDto {
  providerMessageId?: string;
  vendorReference?: string;
  channel?: 'Sms' | 'Email';
  delivered?: boolean;
  detail?: string;
}
