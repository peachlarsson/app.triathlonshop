import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { fetchWarehousesPage } from "../data/api";

export function WarehouseCards() {
  const [page, setPage] = useState(1);
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "warehouses", page],
    queryFn: ({ signal }) => fetchWarehousesPage(page, signal),
  });

  return (
    <section
      aria-label="Warehouses"
      className="space-y-4"
      aria-busy={isPending}
    >
      {error ? (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          <Card>
            <CardHeader>
              <CardTitle className="text-sm font-normal text-muted-foreground">
                Name
              </CardTitle>
              <CardDescription>Location</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <p role="alert" className="text-sm text-destructive">
                {error.message}
              </p>
            </CardContent>
            <CardFooter>
              <Button
                variant="outline"
                className="w-full"
                disabled={isFetching}
                onClick={() => void refetch()}
              >
                Retry
              </Button>
            </CardFooter>
          </Card>
        </div>
      ) : isPending ? (
        <Card>
          <CardContent className="text-center text-sm text-muted-foreground">
            <p role="status">Loading warehouses...</p>
          </CardContent>
        </Card>
      ) : data.data.length ? (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {data.data.map((warehouse) => (
            <Link
              key={warehouse.id}
              to="/stock/warehouse/$warehouseId"
              params={{ warehouseId: String(warehouse.id) }}
              aria-label={`View ${warehouse.warehouseName}`}
              className="min-w-0 rounded-xl focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <Card className="h-full transition-colors hover:bg-accent">
                <CardHeader>
                  <CardTitle className="break-words">
                    {warehouse.warehouseName}
                  </CardTitle>
                  <CardDescription className="break-words">
                    {warehouse.city ?? "City not available"},{" "}
                    {warehouse.countryName ?? "Country not available"}
                  </CardDescription>
                </CardHeader>
              </Card>
            </Link>
          ))}
        </div>
      ) : (
        <Card>
          <CardContent className="text-center text-sm text-muted-foreground">
            No warehouses found.
          </CardContent>
        </Card>
      )}
      {data && !error && (
        <div className="flex flex-wrap items-center justify-between gap-3">
          <p role="status" className="text-sm text-muted-foreground">
            {data.totalCount.toLocaleString()} warehouses
          </p>
          {data.totalPages > 1 && (
            <div className="flex items-center gap-3">
              <Button
                variant="outline"
                size="sm"
                disabled={page === 1 || isFetching}
                onClick={() => setPage((value) => value - 1)}
              >
                Previous
              </Button>
              <span className="text-sm">
                Page {page} of {data.totalPages}
              </span>
              <Button
                variant="outline"
                size="sm"
                disabled={page >= data.totalPages || isFetching}
                onClick={() => setPage((value) => value + 1)}
              >
                Next
              </Button>
            </div>
          )}
        </div>
      )}
    </section>
  );
}
