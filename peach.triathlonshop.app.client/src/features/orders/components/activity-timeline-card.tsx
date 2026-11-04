import type { ReactNode } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export type ActivityTimelineItem = {
  id: number;
  status: string;
  occurredAt: string;
  details?: ReactNode;
};

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
  timeStyle: "short",
});

export function ActivityTimelineCard({
  title,
  items,
}: {
  title: string;
  items: ActivityTimelineItem[];
}) {
  const sortedItems = [...items].sort(
    (first, second) =>
      new Date(second.occurredAt).getTime() -
      new Date(first.occurredAt).getTime(),
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent>
        {sortedItems.length === 0 ? (
          <p className="text-sm text-muted-foreground">No activity yet.</p>
        ) : (
          <ol className="space-y-0">
            {sortedItems.map((item, index) => (
              <li key={item.id} className="relative flex gap-3 pb-5 last:pb-0">
                <div className="relative flex w-3 shrink-0 justify-center">
                  <span className="z-10 mt-1 size-2 rounded-full bg-primary" />
                  {index < sortedItems.length - 1 && (
                    <span
                      aria-hidden="true"
                      className="absolute top-3 bottom-0 w-px bg-border"
                    />
                  )}
                </div>
                <div className="min-w-0 flex-1 space-y-1">
                  <div className="flex flex-wrap items-baseline justify-between gap-x-2 gap-y-1">
                    <p className="font-medium">{item.status}</p>
                    <time
                      className="text-xs text-muted-foreground"
                      dateTime={item.occurredAt}
                    >
                      {dateFormatter.format(new Date(item.occurredAt))}
                    </time>
                  </div>
                  {item.details && (
                    <div className="text-sm text-muted-foreground">
                      {item.details}
                    </div>
                  )}
                </div>
              </li>
            ))}
          </ol>
        )}
      </CardContent>
    </Card>
  );
}
