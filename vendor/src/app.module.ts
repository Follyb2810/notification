import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { VendorController } from './vendor.controller';
import { VendorService } from './vendor.service';

@Module({
  imports: [ConfigModule.forRoot({ isGlobal: true })],
  controllers: [VendorController],
  providers: [VendorService],
})
export class AppModule {}
