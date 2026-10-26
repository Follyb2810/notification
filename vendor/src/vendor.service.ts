import { BadGatewayException, Injectable, ServiceUnavailableException } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { DeliveryReceiptDto, EmailWebhookDto, TextWebhookDto } from './webhook.dto';

@Injectable()
export class VendorService {
  constructor(private readonly config: ConfigService) {}

  sendText(input: TextWebhookDto = {}) {
    return this.forward('/api/webhooks/text-messages', {
      reference: input.reference?.trim() || `vendor-sms-${Date.now()}`,
      phoneNumber: input.phoneNumber?.trim() || '+2348012345678',
      message: input.message?.trim() || 'Your policy renewal is due.',
      senderId: input.senderId?.trim() || 'Notify',
    });
  }

  sendEmail(input: EmailWebhookDto = {}) {
    return this.forward('/api/webhooks/emails', {
      reference: input.reference?.trim() || `vendor-email-${Date.now()}`,
      to: input.to?.trim() || 'client@example.com',
      subject: input.subject?.trim() || 'Policy renewal',
      body: input.body?.trim() || 'Your policy renewal is due.',
      isHtml: input.isHtml ?? false,
    });
  }

  sendDeliveryReceipt(input: DeliveryReceiptDto = {}) {
    return this.forward('/api/webhooks/delivery-receipts', {
      providerMessageId: input.providerMessageId,
      vendorReference: input.vendorReference?.trim() || `vendor-sms-${Date.now()}`,
      channel: input.channel ?? 'Sms',
      delivered: input.delivered ?? true,
      detail: input.detail?.trim() || 'Delivered to handset',
    });
  }

  private async forward(path: string, payload: object) {
    const baseUrl = (this.config.get<string>('NOTIFICATION_API_URL') ?? 'http://localhost:5218').replace(
      /\/$/,
      '',
    );
    const secret = this.config.get<string>('NOTIFICATION_SECRET');
    if (!secret) {
      throw new ServiceUnavailableException('Set NOTIFICATION_SECRET in vendor/.env.');
    }

    const url = `${baseUrl}${path}`;
    let response: Response;
    try {
      response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Notification-Secret': secret,
        },
        body: JSON.stringify(payload),
      });
    } catch (error) {
      const message = error instanceof Error ? error.message : 'The notification API could not be reached.';
      throw new BadGatewayException(message);
    }

    const body: unknown = await response.json().catch(() => null);
    const result = { sentTo: url, payload, status: response.status, body };
    if (!response.ok) {
      throw new BadGatewayException(result);
    }

    return result;
  }
}
