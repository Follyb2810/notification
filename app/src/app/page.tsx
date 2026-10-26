import { DashboardOverview } from "@/components/dashboard/overview"
import { loadDashboard, type OutboundMessage } from "@/lib/notifications"

export default async function HomePage() {
  try {
    const [report, texts, emails] = await loadDashboard()
    const messages = [...texts.items, ...emails.items].sort(
      (left, right) => new Date(right.createdAt).getTime() - new Date(left.createdAt).getTime()
    ) satisfies OutboundMessage[]

    return <DashboardOverview report={report} messages={messages} />
  } catch (error) {
    const message = error instanceof Error ? error.message : "The dashboard could not load."
    return (
      <main className="mx-auto flex min-h-screen max-w-lg flex-col justify-center px-6">
        <p className="text-xs font-medium tracking-[0.18em] text-muted-foreground uppercase">
          Notify
        </p>
        <h1 className="mt-3 text-3xl font-semibold tracking-tight">Dashboard unavailable</h1>
        <p className="mt-3 text-sm leading-6 text-muted-foreground">{message}</p>
        <p className="mt-3 text-sm leading-6 text-muted-foreground">
          Start the API, then set <code>API_BASE_URL</code> and <code>NOTIFICATION_SECRET</code> in{" "}
          <code>app/.env.local</code>.
        </p>
      </main>
    )
  }
}
