import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getRouteApi, Link } from "@tanstack/react-router";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fetchWarehouse, WarehouseNotFoundError } from "./data/api";

const route = getRouteApi("/stock/warehouse/$warehouseId");

export function WarehouseDetails() {
  const { warehouseId } = route.useParams();
  return (
    <WarehouseDetailsContent key={warehouseId} warehouseId={warehouseId} />
  );
}

function WarehouseDetailsContent({ warehouseId }: { warehouseId: string }) {
  const [page, setPage] = useState(1);
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "warehouse", warehouseId, page],
    queryFn: ({ signal }) => fetchWarehouse(warehouseId, page, signal),
    retry: (count, error) =>
      !(error instanceof WarehouseNotFoundError) && count < 2,
  });

  return (
    <>
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>
      <Main className="flex flex-1 flex-col gap-4 sm:gap-6">
        <Button asChild variant="outline" className="self-start">
          <Link to="/stock" hash="warehouses">
            Stock : Warehouses
          </Link>
        </Button>
        {error ? (
          <div className="space-y-2">
            <p role="alert" className="text-sm text-destructive">
              {error.message}
            </p>
            <Button
              variant="outline"
              disabled={isFetching}
              onClick={() => void refetch()}
            >
              Retry
            </Button>
          </div>
        ) : null}
        {isPending && <p role="status">Loading warehouse...</p>}
        <section
          aria-label="Warehouse summary"
          className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4"
        >
          <Card>
            <CardHeader>
              <CardTitle>Warehouse name</CardTitle>
            </CardHeader>
            <CardContent className="text-xl font-semibold">
              {data?.warehouse.warehouseName ??
                (isPending ? "Loading..." : "Not available")}
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Warehouse location</CardTitle>
            </CardHeader>
            <CardContent className="text-xl font-semibold">
              {data?.warehouse.city && data.warehouse.countryName
                ? `${data.warehouse.city}, ${data.warehouse.countryName}`
                : "Not available"}
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Stock status</CardTitle>
            </CardHeader>
            <CardContent className="space-y-2 text-sm">
              {data ? (
                <>
                  <p>
                    <span className="font-semibold">
                      {data.inStockArticles.toLocaleString()}
                    </span>{" "}
                    articles in stock
                  </p>
                  <p>
                    <span className="font-semibold">
                      {data.outOfStockArticles.toLocaleString()}
                    </span>{" "}
                    articles out of stock
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
              <CardTitle>Orders shipped</CardTitle>
              <CardDescription>
                All-time shipped orders from this warehouse
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-2">
              <p className="text-2xl font-bold">
                {data?.shippedOrders?.toLocaleString() ?? "Not available"}
              </p>
            </CardContent>
          </Card>
        </section>
        {data && !error && (
          <section
            aria-label="Warehouse articles"
            className="overflow-hidden rounded-lg border bg-card"
          >
            <Table aria-label="Warehouse articles">
              <TableHeader>
                <TableRow>
                  <TableHead>Article number</TableHead>
                  <TableHead>Article</TableHead>
                  <TableHead>Quantity</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data.articles.data.length ? (
                  data.articles.data.map((article) => (
                    <TableRow key={article.id}>
                      <TableCell>{article.number}</TableCell>
                      <TableCell className="font-medium">
                        {article.name}
                      </TableCell>
                      <TableCell>{article.quantity.toLocaleString()}</TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={3} className="h-24 text-center">
                      No articles assigned to this warehouse.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            <div className="flex flex-wrap items-center justify-between gap-3 border-t p-4">
              <p role="status" className="text-sm text-muted-foreground">
                {data.articles.totalCount.toLocaleString()} assigned articles
              </p>
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
                  Page {page} of {Math.max(1, data.articles.totalPages)}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page >= data.articles.totalPages || isFetching}
                  onClick={() => setPage((value) => value + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          </section>
        )}
      </Main>
    </>
  );
}
