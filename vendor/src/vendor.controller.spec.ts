import { Test, TestingModule } from '@nestjs/testing';
import { VendorController } from './vendor.controller';
import { VendorService } from './vendor.service';

describe('VendorController', () => {
  let controller: VendorController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [VendorController],
      providers: [
        {
          provide: VendorService,
          useValue: {},
        },
      ],
    }).compile();

    controller = module.get(VendorController);
  });

  it('lists the webhook routes', () => {
    expect(controller.index()).toEqual({
      name: 'vendor',
      text: 'POST /webhooks/text-messages',
      email: 'POST /webhooks/emails',
      deliveryReceipt: 'POST /webhooks/delivery-receipts',
    });
  });
});
