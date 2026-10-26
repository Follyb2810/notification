# Local vendor

NestJS stand-in for the core vendor. It sends the text, email, and delivery-receipt webhooks to the notification API so they can be tested on this machine.

```bash
npm run start:dev
```

The server listens on port 4000. `NOTIFICATION_API_URL` and `NOTIFICATION_SECRET` are in `.env` and must match the API. Calls are listed in `vendor.http`.
