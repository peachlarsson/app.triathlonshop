import { X } from "lucide-react";
import { Button } from "@/components/ui/button";

type WarehouseErrorPopupProps = {
  message: string;
  onDismiss: () => void;
};

export function WarehouseErrorPopup({
  message,
  onDismiss,
}: WarehouseErrorPopupProps) {
  return (
    <div
      role="alert"
      className="flex w-[min(24rem,calc(100vw-2rem))] items-start gap-3 rounded-lg border border-destructive/30 bg-background p-4 text-foreground shadow-lg"
    >
      <div className="min-w-0 flex-1">
        <p className="text-sm font-semibold">Warehouse locations unavailable</p>
        <p className="mt-1 break-words text-sm text-muted-foreground">
          {message}
        </p>
      </div>
      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="size-7"
        aria-label="Dismiss error"
        onClick={onDismiss}
      >
        <X aria-hidden="true" />
      </Button>
    </div>
  );
}
