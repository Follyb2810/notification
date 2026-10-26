import { Badge } from "@/components/ui/badge"
import type { MessageStatus } from "@/lib/notifications"
import { cn } from "cn"

const statusClass: Record<MessageStatus, string> = {
  Delivered: "bg-emerald-500/15 text-emerald-700 dark:text-emerald-300",
  Sent: "bg-sky-500/15 text-sky-700 dark:text-sky-300",
  Failed: "bg-rose-500/15 text-rose-700 dark:text-rose-300",
  Sending: "bg-amber-500/15 text-amber-800 dark:text-amber-200",
  Received: "bg-muted text-muted-foreground",
}

export function StatusBadge({ status }: { status: MessageStatus }) {
  return (
    <Badge variant="outline" className={cn("border-transparent", statusClass[status])}>
      {status}
    </Badge>
  )
}
