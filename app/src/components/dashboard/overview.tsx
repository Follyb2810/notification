import {
  AlertTriangle,
  CheckCheck,
  Clock3,
  Mail,
  MessageSquareText,
  Send,
} from "lucide-react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { StatusBadge } from "@/components/dashboard/status-badge"
import type { MessageReport, MessageSummary, OutboundMessage } from "@/lib/notifications"

const dateFormat = new Intl.DateTimeFormat("en-GB", {
  day: "2-digit",
  month: "short",
  hour: "2-digit",
  minute: "2-digit",
})

function formatWhen(value: string | null) {
  if (!value) return "—"
  return dateFormat.format(new Date(value))
}

function share(part: number, total: number) {
  if (total <= 0) return 0
  return Math.round((part / total) * 100)
}

export function DashboardOverview({
  report,
  messages,
}: {
  report: MessageReport
  messages: OutboundMessage[]
}) {
  const totals = report.all

  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto grid min-h-screen max-w-[1400px] lg:grid-cols-[240px_1fr]">
        <aside className="border-b bg-card/70 px-5 py-6 lg:border-r lg:border-b-0">
          <div className="flex items-center gap-3">
            <div className="grid size-10 place-items-center rounded-2xl bg-foreground text-background">
              <Send className="size-4" />
            </div>
            <div>
              <p className="text-sm font-semibold tracking-tight">Notify</p>
              <p className="text-xs text-muted-foreground">Delivery desk</p>
            </div>
          </div>
          <nav className="mt-8">
            <p className="rounded-xl bg-foreground px-3 py-2 text-sm text-background">Overview</p>
          </nav>
          <p className="mt-8 text-xs leading-5 text-muted-foreground">
            Counts come from the notification database. The API secret stays in the server env.
          </p>
        </aside>

        <main className="px-5 py-6 sm:px-8">
          <div className="flex flex-wrap items-end justify-between gap-4">
            <div>
              <p className="text-xs font-medium tracking-[0.18em] text-muted-foreground uppercase">
                Live from the database
              </p>
              <h1 className="mt-2 text-3xl font-semibold tracking-tight">What has gone out</h1>
            </div>
            <a
              href="/"
              className="rounded-full border px-4 py-2 text-sm hover:bg-muted"
            >
              Refresh
            </a>
          </div>

          <section className="mt-6 overflow-hidden rounded-3xl border bg-card">
            <div className="grid gap-6 bg-[radial-gradient(circle_at_top_left,rgba(14,165,233,0.18),transparent_36%),radial-gradient(circle_at_bottom_right,rgba(16,185,129,0.16),transparent_32%)] p-6 sm:grid-cols-[1.4fr_1fr] sm:p-8">
              <div>
                <p className="text-sm text-muted-foreground">Sent out</p>
                <p className="mt-2 text-6xl font-semibold tracking-tight tabular-nums">
                  {totals.sentOut.toLocaleString()}
                </p>
                <p className="mt-2 max-w-md text-sm text-muted-foreground">
                  {totals.total.toLocaleString()} messages are stored. Sent out is every text and
                  email marked sent or delivered.
                </p>
              </div>
              <div className="grid grid-cols-2 gap-3 self-end">
                <MiniStat label="Text" value={report.textMessages.sentOut} />
                <MiniStat label="Email" value={report.emails.sentOut} />
              </div>
            </div>
          </section>

          <section className="mt-4 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
            <Metric icon={Send} label="Sent" value={totals.sent} hint="Accepted, delivery not confirmed" />
            <Metric icon={CheckCheck} label="Delivered" value={totals.delivered} hint="Receipt confirmed" />
            <Metric icon={AlertTriangle} label="Failed" value={totals.failed} hint="Saved, not delivered" />
            <Metric
              icon={Clock3}
              label="In progress"
              value={totals.received + totals.sending}
              hint="Received or still sending"
            />
          </section>

          <section className="mt-4 grid gap-3 lg:grid-cols-2">
            <ChannelCard
              icon={MessageSquareText}
              title="Text messages"
              summary={report.textMessages}
            />
            <ChannelCard icon={Mail} title="Email" summary={report.emails} />
          </section>

          <Card className="mt-4">
            <CardHeader>
              <CardTitle>Recent messages</CardTitle>
              <CardDescription>Latest text messages and emails stored by the API.</CardDescription>
            </CardHeader>
            <CardContent>
              {messages.length === 0 ? (
                <p className="py-8 text-sm text-muted-foreground">
                  No messages are stored yet. A vendor webhook will show up here after it is saved.
                </p>
              ) : (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>When</TableHead>
                      <TableHead>Channel</TableHead>
                      <TableHead>Recipient</TableHead>
                      <TableHead>Reference</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Message</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {messages.map((message) => (
                      <TableRow key={message.id}>
                        <TableCell className="whitespace-nowrap">
                          {formatWhen(message.createdAt)}
                        </TableCell>
                        <TableCell>{message.channel === "Sms" ? "Text" : "Email"}</TableCell>
                        <TableCell className="max-w-[180px] truncate">{message.recipient}</TableCell>
                        <TableCell className="max-w-[160px] truncate font-mono text-xs">
                          {message.vendorReference}
                        </TableCell>
                        <TableCell>
                          <StatusBadge status={message.status} />
                        </TableCell>
                        <TableCell className="max-w-[280px] truncate text-muted-foreground">
                          {message.subject ? `${message.subject} — ` : ""}
                          {message.body}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              )}
            </CardContent>
          </Card>
        </main>
      </div>
    </div>
  )
}

function MiniStat({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-2xl border bg-background/70 p-4">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 text-2xl font-semibold tabular-nums">{value.toLocaleString()}</p>
    </div>
  )
}

function Metric({
  icon: Icon,
  label,
  value,
  hint,
}: {
  icon: typeof Send
  label: string
  value: number
  hint: string
}) {
  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <CardDescription>{label}</CardDescription>
          <Icon className="size-4 text-muted-foreground" />
        </div>
        <CardTitle className="text-3xl tabular-nums">{value.toLocaleString()}</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-xs text-muted-foreground">{hint}</p>
      </CardContent>
    </Card>
  )
}

function ChannelCard({
  icon: Icon,
  title,
  summary,
}: {
  icon: typeof Mail
  title: string
  summary: MessageSummary
}) {
  const rows = [
    ["Delivered", summary.delivered],
    ["Sent", summary.sent],
    ["Failed", summary.failed],
    ["In progress", summary.received + summary.sending],
  ] as const

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center gap-2">
          <Icon className="size-4" />
          <CardTitle>{title}</CardTitle>
        </div>
        <CardDescription>{summary.total.toLocaleString()} stored</CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
        {rows.map(([label, value]) => (
          <div key={label}>
            <div className="mb-1 flex justify-between text-xs">
              <span>{label}</span>
              <span className="tabular-nums text-muted-foreground">
                {value.toLocaleString()} · {share(value, summary.total)}%
              </span>
            </div>
            <div className="h-1.5 overflow-hidden rounded-full bg-muted">
              <div
                className="h-full rounded-full bg-foreground"
                style={{ width: `${share(value, summary.total)}%` }}
              />
            </div>
          </div>
        ))}
        <Separator />
        <p className="text-sm">
          <span className="font-medium tabular-nums">{summary.sentOut.toLocaleString()}</span>
          <span className="text-muted-foreground"> sent out</span>
        </p>
      </CardContent>
    </Card>
  )
}
