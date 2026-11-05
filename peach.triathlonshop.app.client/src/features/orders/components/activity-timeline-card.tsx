import { useEffect, useRef, useState } from "react";
import type { ReactNode } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export type ActivityTimelineItem = {
  id: number;
  status: string;
  occurredAt: string;
  changeKey: string;
  details?: ReactNode;
};

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
  timeStyle: "short",
});

export function ActivityTimelineCard({
  title,
  items,
  updateVersion,
}: {
  title: string;
  items: ActivityTimelineItem[];
  updateVersion: number;
}) {
  const [highlightedIds, setHighlightedIds] = useState<Set<number>>(
    () => new Set(),
  );
  const previousItems = useRef<Map<number, string> | null>(null);
  const highlightTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);
  const pendingUpdateTimeout = useRef<ReturnType<typeof setTimeout> | null>(
    null,
  );
  const previousUpdateVersion = useRef(updateVersion);
  const awaitingUpdate = useRef(false);

  useEffect(() => {
    const currentItems = new Map(
      items.map((item) => [item.id, item.changeKey]),
    );
    const previous = previousItems.current;
    previousItems.current = currentItems;
    const updateReceived = previousUpdateVersion.current !== updateVersion;
    previousUpdateVersion.current = updateVersion;

    if (previous === null) return;

    const changedIds = items
      .filter((item) => previous.get(item.id) !== item.changeKey)
      .map((item) => item.id);

    if (updateReceived) {
      awaitingUpdate.current = true;
      if (pendingUpdateTimeout.current) {
        clearTimeout(pendingUpdateTimeout.current);
      }
      pendingUpdateTimeout.current = setTimeout(() => {
        awaitingUpdate.current = false;
        pendingUpdateTimeout.current = null;
      }, 10_000);
    }

    if (!awaitingUpdate.current || changedIds.length === 0) return;

    awaitingUpdate.current = false;
    if (pendingUpdateTimeout.current) {
      clearTimeout(pendingUpdateTimeout.current);
      pendingUpdateTimeout.current = null;
    }
    setHighlightedIds(new Set(changedIds));
    if (highlightTimeout.current) clearTimeout(highlightTimeout.current);
    highlightTimeout.current = setTimeout(() => {
      setHighlightedIds(new Set());
      highlightTimeout.current = null;
    }, 2000);
  }, [items, updateVersion]);

  useEffect(
    () => () => {
      if (highlightTimeout.current) clearTimeout(highlightTimeout.current);
      if (pendingUpdateTimeout.current) {
        clearTimeout(pendingUpdateTimeout.current);
      }
    },
    [],
  );

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
              <li
                key={item.id}
                className={`relative -mx-2 flex gap-3 rounded-md px-2 pb-5 transition-colors duration-300 last:pb-0 ${
                  highlightedIds.has(item.id) ? "bg-green-500/15" : ""
                }`}
              >
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
