import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { Info } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { fetchOverview } from "../data/api";

export function StockOverview() {
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "overview"],
    queryFn: ({ signal }) => fetchOverview(signal),
  });
  const shippedWarehouses = [...(data?.warehouses ?? [])].sort(
    (a, b) =>
      (b.shipped ?? 0) - (a.shipped ?? 0) ||
      (a.name ?? "").localeCompare(b.name ?? ""),
  );
  const mostShippedWarehouses = shippedWarehouses;
  const leastShippedWarehouses = [...shippedWarehouses].reverse();

  return (
    <section aria-label="Stock overview" className="space-y-4">
      <div>
        <h3 className="font-semibold">Across all warehouses</h3>
      </div>
      {error ? (
        <div className="space-y-2">
          <p role="alert" className="text-sm text-destructive">
            {error.message}
          </p>
          <Button
            variant="outline"
            onClick={() => void refetch()}
            disabled={isFetching}
          >
            Retry overview
          </Button>
        </div>
      ) : null}
      {isPending && (
        <p role="status" className="text-sm text-muted-foreground">
          Loading stock overview...
        </p>
      )}
      <div className="grid gap-4 sm:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Total warehouses</CardTitle>
            <CardDescription>
              Locations, stock, and outgoing orders
            </CardDescription>
          </CardHeader>
          <CardContent className="min-w-0 space-y-3 pr-2">
            <p className="text-2xl font-bold">
              {data?.warehouses.length.toLocaleString() ??
                (isPending ? "Loading..." : "Not available")}
            </p>
            {data?.warehouses.length ? (
              <div className="overflow-x-auto">
                <div className="grid w-full min-w-[26rem] grid-cols-[minmax(5rem,1fr)_3rem_3.5rem_3.5rem_3.5rem_3.5rem] items-center gap-x-2 overflow-y-hidden text-[10px] leading-tight text-muted-foreground [scrollbar-gutter:stable] sm:text-xs">
                  <span className="min-w-0 text-left">Warehouse</span>
                  <span className="text-center">In stock</span>
                  <span className="text-center">Out of stock</span>
                  <span className="text-center">Total Articles</span>
                  <span className="border-l pl-2 text-center">
                    Awaiting dispatch
                  </span>
                  <span className="flex flex-col items-center text-center">
                    Shipped
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <button
                          type="button"
                          aria-label="About shipped count"
                          className="inline-flex size-4 items-center justify-center rounded-full focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                        >
                          <Info aria-hidden="true" className="size-3" />
                        </button>
                      </TooltipTrigger>
                      <TooltipContent>
                        Shipped orders in the last 30 days
                      </TooltipContent>
                    </Tooltip>
                  </span>
                </div>
                <ul className="max-h-48 space-y-2 overflow-y-auto pt-2 text-sm [scrollbar-gutter:stable]">
                  {data.warehouses.map((warehouse) => (
                    <li
                      key={warehouse.id}
                      className="grid w-full min-w-[26rem] grid-cols-[minmax(5rem,1fr)_3rem_3.5rem_3.5rem_3.5rem_3.5rem] gap-x-2"
                    >
                      <span className="min-w-0 text-left">
                        <Link
                          to="/stock/warehouse/$warehouseId"
                          params={{
                            warehouseId: String(warehouse.id),
                          }}
                          className="block truncate font-medium hover:underline"
                        >
                          {warehouse.name ?? `Warehouse ${warehouse.id}`}
                        </Link>
                      </span>
                      <span className="text-center">
                        {warehouse.inStock?.toLocaleString() ?? "—"}
                      </span>
                      <span className="text-center">
                        {warehouse.outOfStock?.toLocaleString() ?? "—"}
                      </span>
                      <span className="text-center">
                        {warehouse.totalArticles?.toLocaleString() ?? "—"}
                      </span>
                      <span className="border-l pl-2 text-center">
                        {warehouse.awaitingDispatch?.toLocaleString() ?? "—"}
                      </span>
                      <span className="text-center">
                        {warehouse.shipped?.toLocaleString() ?? "—"}
                      </span>
                    </li>
                  ))}
                </ul>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">
                {data
                  ? "No warehouse stock status available."
                  : "Loading warehouse details..."}
              </p>
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Warehouse shipments</CardTitle>
            <CardDescription>
              Highest and lowest shipped order counts
            </CardDescription>
          </CardHeader>
          <CardContent className="min-w-0 space-y-3">
            {mostShippedWarehouses.length ? (
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="min-w-0">
                  <p className="mb-2 text-sm font-semibold">Most shipped</p>
                  <div className="mb-2 grid grid-cols-[minmax(0,1fr)_5rem] gap-x-2 text-xs text-muted-foreground">
                    <span>Warehouse</span>
                    <span className="text-right">Shipped</span>
                  </div>
                  <ol className="max-h-48 space-y-2 overflow-y-auto text-sm">
                    {mostShippedWarehouses.map((warehouse, index) => (
                      <li
                        key={warehouse.id}
                        className="grid grid-cols-[minmax(0,1fr)_5rem] gap-x-2"
                      >
                        <span className="flex min-w-0 gap-2">
                          <span className="shrink-0 text-muted-foreground">
                            {index + 1}.
                          </span>
                          <Link
                            to="/stock/warehouse/$warehouseId"
                            params={{
                              warehouseId: String(warehouse.id),
                            }}
                            className="min-w-0 truncate font-medium hover:underline"
                          >
                            {warehouse.name ?? `Warehouse ${warehouse.id}`}
                          </Link>
                        </span>
                        <span className="text-right">
                          {warehouse.shipped?.toLocaleString() ?? "—"}
                        </span>
                      </li>
                    ))}
                  </ol>
                </div>
                <div className="min-w-0 border-t pt-3 sm:border-l sm:border-t-0 sm:pl-4 sm:pt-0">
                  <p className="mb-2 text-sm font-semibold">Least shipped</p>
                  <div className="mb-2 grid grid-cols-[minmax(0,1fr)_5rem] gap-x-2 text-xs text-muted-foreground">
                    <span>Warehouse</span>
                    <span className="text-right">Shipped</span>
                  </div>
                  <ol className="max-h-48 space-y-2 overflow-y-auto text-sm">
                    {leastShippedWarehouses.map((warehouse, index) => (
                      <li
                        key={warehouse.id}
                        className="grid grid-cols-[minmax(0,1fr)_5rem] gap-x-2"
                      >
                        <span className="flex min-w-0 gap-2">
                          <span className="shrink-0 text-muted-foreground">
                            {index + 1}.
                          </span>
                          <Link
                            to="/stock/warehouse/$warehouseId"
                            params={{
                              warehouseId: String(warehouse.id),
                            }}
                            className="min-w-0 truncate font-medium hover:underline"
                          >
                            {warehouse.name ?? `Warehouse ${warehouse.id}`}
                          </Link>
                        </span>
                        <span className="text-right">
                          {warehouse.shipped?.toLocaleString() ?? "—"}
                        </span>
                      </li>
                    ))}
                  </ol>
                </div>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">
                No shipment data available.
              </p>
            )}
          </CardContent>
        </Card>
      </div>
    </section>
  );
}
