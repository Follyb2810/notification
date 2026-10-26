export type MessageStatus = "Received" | "Sending" | "Sent" | "Delivered" | "Failed"

export type MessageChannel = "Sms" | "Email"

export type MessageSummary = {
  total: number
  received: number
  sending: number
  sent: number
  delivered: number
  failed: number
  sentOut: number
}

export type MessageReport = {
  textMessages: MessageSummary
  emails: MessageSummary
  all: MessageSummary
}

export type OutboundMessage = {
  id: string
  channel: MessageChannel
  vendorReference: string
  recipient: string
  subject: string | null
  body: string
  status: MessageStatus
  failureReason: string | null
  createdAt: string
  sentAt: string | null
  deliveredAt: string | null
}

export type PagedMessages = {
  items: OutboundMessage[]
  totalCount: number
  page: number
  pageSize: number
}

type ApiResult<T> = {
  success: boolean
  message: string
  data: T | null
}

function requireEnv(name: string) {
  const value = process.env[name]
  if (!value) {
    throw new Error(`Set ${name} in app/.env.local.`)
  }
  return value
}

async function getJson<T>(path: string): Promise<T> {
  const baseUrl = requireEnv("API_BASE_URL").replace(/\/$/, "")
  const secret = requireEnv("NOTIFICATION_SECRET")
  const response = await fetch(`${baseUrl}${path}`, {
    headers: { "X-Notification-Secret": secret },
    cache: "no-store",
  })

  if (!response.ok) {
    throw new Error(`The notification API returned ${response.status}.`)
  }

  const body = (await response.json()) as ApiResult<T>
  if (!body.success || body.data === null) {
    throw new Error(body.message || "The notification API rejected the request.")
  }

  return body.data
}

export function loadDashboard() {
  return Promise.all([
    getJson<MessageReport>("/api/message-reports/summary"),
    getJson<PagedMessages>("/api/text-messages?page=1&pageSize=8"),
    getJson<PagedMessages>("/api/emails?page=1&pageSize=8"),
  ])
}
