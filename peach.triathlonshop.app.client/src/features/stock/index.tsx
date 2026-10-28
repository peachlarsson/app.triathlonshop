import { useEffect, useState, type ReactNode } from "react";
import { useQuery } from "@tanstack/react-query";
import { useLocation, useNavigate } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { StockOverview } from "./components/stock-overview";
import { StockChart } from "./components/stock-chart";
import { WarehouseCards } from "./components/warehouse-cards";
import { fetchArticlesPage } from "./data/api";
import { SortDirection, SortKey } from "./data/types";
import type { ArticleDto, GetArticlesRequest, PaginatedResponse } from "./data/types";

const currency = new Intl.NumberFormat("sv-SE", {
  style: "currency",
  currency: "SEK",
});

export function Stock() {
  const hash = useLocation({ select: (location) => location.hash });
  const navigate = useNavigate();
  useEffect(() => {
    if (!hash) {
      void navigate({
        hash: "overview",
        replace: true,
        resetScroll: false,
        hashScrollIntoView: false,
      });
    }
  }, [hash, navigate]);
  const activeTab =
    hash === "warehouses" || hash === "articles" ? hash : "overview";

  return (
    <>
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>
      <Main className="flex flex-1 flex-col gap-4 sm:gap-6">
        <div className="mb-2 flex flex-wrap items-end justify-between gap-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Stock</h2>
          </div>
        </div>

        <Tabs
          orientation="vertical"
          value={activeTab}
          onValueChange={(value) => {
            void navigate({
              hash: value,
              resetScroll: false,
              hashScrollIntoView: false,
            });
          }}
          className="space-y-4"
        >
          <div className="w-full overflow-x-auto pb-2">
            <TabsList>
              <TabsTrigger value="overview">Overview</TabsTrigger>
              <TabsTrigger value="warehouses">Warehouses</TabsTrigger>
              <TabsTrigger value="articles">Articles</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="overview" className="space-y-4">
            <StockOverview />
            <StockChart />
          </TabsContent>

          <TabsContent value="warehouses" className="space-y-4">
            <WarehouseCards />
          </TabsContent>

          <TabsContent value="articles" className="space-y-4">
            <InventoryTable<ArticleDto>
              loadPage={fetchArticlesPage}
              label="articles"
              columns={["Article name", "Article number", "Unit price"]}
              renderRow={(item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.articleName}</TableCell>
                  <TableCell>{item.articleNumber}</TableCell>
                  <TableCell>{currency.format(item.unitPrice)}</TableCell>
                </TableRow>
              )}
            />
          </TabsContent>
        </Tabs>
      </Main>
    </>
  );
}

function InventoryTable<T>({
  loadPage,
  label,
  columns,
  renderRow,
}: {
  loadPage: (
    request: GetArticlesRequest,
    signal: AbortSignal,
  ) => Promise<PaginatedResponse<T>>;
  label: string;
  columns: string[];
  renderRow: (item: T) => ReactNode;
}) {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", label, page],
    queryFn: ({ signal }) => loadPage({
        articleName: "",
        pageNumber: page,
        pageSize: pageSize,
        sortKey: SortKey.ArticleName,
        sortDirection: SortDirection.Asc
      },
      signal
    )
  });

  return (
    <section
      aria-label={label}
      className="overflow-hidden rounded-lg border bg-card"
    >
      <Table aria-label={label} aria-busy={isPending}>
        <TableHeader>
          <TableRow>
            {columns.map((column) => (
              <TableHead key={column}>{column}</TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {error ? (
            <TableRow>
              <TableCell
                colSpan={columns.length}
                className="h-24 text-center text-muted-foreground"
              >
                <div className="space-y-2 py-4">
                  <p role="alert" className="text-sm text-destructive">
                    {error.message}
                  </p>
                  <Button
                    variant="outline"
                    onClick={() => void refetch()}
                    disabled={isFetching}
                  >
                    Retry
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          ) : isPending ? (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                <span role="status">Loading {label}...</span>
              </TableCell>
            </TableRow>
          ) : data && data.data.length > 0 ? (
            data.data.map(renderRow)
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                No {label} found.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>

      <div className="flex flex-wrap items-center justify-between gap-3 border-t p-4">
        <p className="text-sm text-muted-foreground" role="status">
          {error
            ? "Not available"
            : data
              ? `${data.totalCount.toLocaleString()} ${
                  label === "stock" ? "stock entries" : label
                }`
              : "Loading..."}
        </p>
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="sm"
            disabled={page === 1 || isFetching || !!error}
            onClick={() => setPage((value) => value - 1)}
          >
            Previous
          </Button>
          <span className="text-sm">
            Page {page} of {Math.max(1, data?.totalPages ?? 1)}
          </span>
          <Button
            variant="outline"
            size="sm"
            disabled={!data || page >= data.totalPages || isFetching || !!error}
            onClick={() => setPage((value) => value + 1)}
          >
            Next
          </Button>
        </div>
      </div>
    </section>
  );
}
