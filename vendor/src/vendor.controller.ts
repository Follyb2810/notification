import { Body, Controller, Get, Post } from '@nestjs/common';
import { DeliveryReceiptDto, EmailWebhookDto, TextWebhookDto } from './webhook.dto';
import { VendorService } from './vendor.service';

@Controller()
export class VendorController {
  constructor(private readonly vendor: VendorService) {}

  @Get()
  index() {
    return {
      name: 'vendor',
      text: 'POST /webhooks/text-messages',
      email: 'POST /webhooks/emails',
      deliveryReceipt: 'POST /webhooks/delivery-receipts',
    };
  }

  @Post('webhooks/text-messages')
  sendText(@Body() body: TextWebhookDto) {
    return this.vendor.sendText(body ?? {});
  }

  @Post('webhooks/emails')
  sendEmail(@Body() body: EmailWebhookDto) {
    return this.vendor.sendEmail(body ?? {});
  }

  @Post('webhooks/delivery-receipts')
  sendDeliveryReceipt(@Body() body: DeliveryReceiptDto) {
    return this.vendor.sendDeliveryReceipt(body ?? {});
  }
}
