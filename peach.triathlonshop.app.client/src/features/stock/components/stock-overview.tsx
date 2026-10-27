import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { fetchOverview } from "../data/api";

export function StockOverview() {
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "overview"],
    queryFn: ({ signal }) => fetchOverview(signal),
  });

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
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <Card>
          <CardHeader>
            <CardTitle>Total warehouses</CardTitle>
            <CardDescription>Reported by the stock API</CardDescription>
          </CardHeader>
          <CardContent className="text-2xl font-bold">
            {data?.totalWarehouses.toLocaleString() ??
              (isPending ? "Loading..." : "Not available")}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Status</CardTitle>
            <CardDescription>Article stock status</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            {data ? (
              <>
                <p>
                  <span className="font-semibold">
                    {data.inStockArticles.toLocaleString()}
                  </span>{" "}
                  in stock
                </p>
                <p>
                  <span className="font-semibold">
                    {data.outOfStockArticles.toLocaleString()}
                  </span>{" "}
                  out of stock
                </p>
              </>
            ) : (
              <p className="text-muted-foreground">
                {isPending ? "Loading..." : "Not available"}
              </p>
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Locations</CardTitle>
            <CardDescription>Warehouse cities and countries</CardDescription>
          </CardHeader>
          <CardContent>
            {data?.locations?.length ? (
              <ul className="max-h-40 space-y-1 overflow-y-auto text-sm">
                {data.locations.map(({ city, countryName }) => (
                  <li key={JSON.stringify([city, countryName])}>
                    {city}, {countryName}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-muted-foreground">
                No data available.
              </p>
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Outgoing orders</CardTitle>
            <CardDescription>By warehouse</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            {data?.outgoingOrders?.length ? (
              <ul className="max-h-60 space-y-3 overflow-y-auto">
                {data.outgoingOrders.map((warehouse) => (
                  <li key={warehouse.warehouseId}>
                    <Link
                      to="/stock/warehouse/$warehouseId"
                      params={{ warehouseId: String(warehouse.warehouseId) }}
                      className="font-semibold hover:underline"
                    >
                      {warehouse.warehouseName}
                    </Link>
                    <p>
                      {warehouse.awaitingDispatch.toLocaleString()} awaiting
                      dispatch
                    </p>
                    <p>{warehouse.shipped.toLocaleString()} shipped</p>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-muted-foreground">
                No data available.
              </p>
            )}
          </CardContent>
        </Card>
      </div>
    </section>
  );
}
