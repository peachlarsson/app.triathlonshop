import { Truck } from "lucide-react";

type LogisticsHeaderProps = {
  totalOrders: number;
  isLoading: boolean;
  hasError: boolean;
};

export function LogisticsHeader({
  totalOrders,
  isLoading,
  hasError,
}: LogisticsHeaderProps) {
  return (
    <div className="flex flex-wrap items-end justify-between gap-3">
      <div>
        <h2 className="text-2xl font-bold tracking-tight">Logistics</h2>
      </div>
      <div className="flex items-center gap-2 rounded-lg border bg-card px-4 py-3">
        <Truck className="size-5 text-muted-foreground" aria-hidden="true" />
        <span className="text-sm text-muted-foreground">Total orders</span>
        <span className="font-semibold tabular-nums">
          {isLoading ? "…" : hasError ? "—" : totalOrders}
        </span>
      </div>
    </div>
  );
}
