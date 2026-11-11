import { Fragment, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { ChevronDown, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fetchArticleWarehouseStock, fetchArticlesPage } from "../data/api";
import { SortDirection, SortKey } from "../data/types";

const currency = new Intl.NumberFormat("sv-SE", {
  style: "currency",
  currency: "SEK",
});

export function ArticlesTable() {
  const [page, setPage] = useState(1);
  const [expandedArticleId, setExpandedArticleId] = useState<number | null>(
    null,
  );
  const pageSize = 10;
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "articles", page],
    queryFn: ({ signal }) =>
      fetchArticlesPage(
        {
          articleName: "",
          pageNumber: page,
          pageSize,
          sortKey: SortKey.ArticleName,
          sortDirection: SortDirection.Asc,
        },
        signal,
      ),
  });
  const warehouseStock = useQuery({
    queryKey: ["inventory", "article-warehouse-stock"],
    queryFn: ({ signal }) => fetchArticleWarehouseStock(signal),
    staleTime: 5 * 60 * 1000,
  });
  const stockByArticle = new Map<number, Map<number, number>>();
  for (const stock of warehouseStock.data?.quantities ?? []) {
    let quantities = stockByArticle.get(stock.articleId);
    if (!quantities) {
      quantities = new Map();
      stockByArticle.set(stock.articleId, quantities);
    }
    quantities.set(
      stock.warehouseId,
      (quantities.get(stock.warehouseId) ?? 0) + stock.quantity,
    );
  }
  const stockSummaryByArticle = new Map(
    [...stockByArticle].map(([articleId, quantities]) => [
      articleId,
      {
        total: [...quantities.values()].reduce(
          (sum, quantity) => sum + quantity,
          0,
        ),
        stockedWarehouses: [...quantities.values()].filter(
          (quantity) => quantity > 0,
        ).length,
      },
    ]),
  );
  const quantitiesByWarehouse = new Map(
    stockByArticle.get(expandedArticleId ?? -1),
  );
  const totalQuantity = [...quantitiesByWarehouse.values()].reduce(
    (total, quantity) => total + quantity,
    0,
  );

  return (
    <section
      aria-label="articles"
      className="overflow-hidden rounded-lg border bg-card"
    >
      <Table
        aria-label="articles"
        aria-busy={isPending || warehouseStock.isPending}
      >
        <TableHeader>
          <TableRow>
            <TableHead>Article name</TableHead>
            <TableHead>Stock</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {warehouseStock.error && expandedArticleId === null && (
            <TableRow>
              <TableCell colSpan={2}>
                <div className="flex flex-wrap items-center justify-between gap-3">
                  <p role="alert" className="text-sm text-destructive">
                    Unable to load warehouse stock:{" "}
                    {warehouseStock.error.message}
                  </p>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => void warehouseStock.refetch()}
                    disabled={warehouseStock.isFetching}
                  >
                    Retry
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          )}
          {error ? (
            <TableRow>
              <TableCell
                colSpan={2}
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
              <TableCell colSpan={2} className="h-24 text-center">
                <span role="status">Loading articles...</span>
              </TableCell>
            </TableRow>
          ) : data && data.data.length > 0 ? (
            data.data.map((article) => (
              <Fragment key={article.id}>
                <TableRow>
                  <TableCell className="font-medium">
                    <div className="flex items-center gap-2">
                      <button
                        type="button"
                        aria-label={`${expandedArticleId === article.id ? "Hide" : "Show"} stock by warehouse for ${article.articleName}`}
                        aria-expanded={expandedArticleId === article.id}
                        aria-controls={`article-stock-${article.id}`}
                        className="inline-flex size-6 shrink-0 items-center justify-center rounded hover:bg-muted focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                        onClick={() =>
                          setExpandedArticleId((current) =>
                            current === article.id ? null : article.id,
                          )
                        }
                      >
                        {expandedArticleId === article.id ? (
                          <ChevronDown aria-hidden="true" className="size-4" />
                        ) : (
                          <ChevronRight aria-hidden="true" className="size-4" />
                        )}
                      </button>
                      <span>{article.articleName}</span>
                    </div>
                  </TableCell>
                  <TableCell>
                    {warehouseStock.data ? (
                      <div className="min-w-36 space-y-1">
                        <p className="text-xs text-muted-foreground">
                          {(
                            stockSummaryByArticle.get(article.id)?.total ?? 0
                          ).toLocaleString()}{" "}
                          units ·{" "}
                          {stockSummaryByArticle.get(article.id)
                            ?.stockedWarehouses ?? 0}
                          /{warehouseStock.data.warehouses.length} warehouses
                        </p>
                      </div>
                    ) : warehouseStock.isPending ? (
                      <span className="text-xs text-muted-foreground">
                        Loading...
                      </span>
                    ) : (
                      "—"
                    )}
                  </TableCell>
                </TableRow>
                {expandedArticleId === article.id && (
                  <TableRow>
                    <TableCell
                      id={`article-stock-${article.id}`}
                      colSpan={2}
                      className="bg-muted/30 p-4"
                    >
                      {warehouseStock.error ? (
                        <div className="flex flex-wrap items-center justify-between gap-3">
                          <p role="alert" className="text-sm text-destructive">
                            {warehouseStock.error.message}
                          </p>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => void warehouseStock.refetch()}
                            disabled={warehouseStock.isFetching}
                          >
                            Retry
                          </Button>
                        </div>
                      ) : warehouseStock.isPending ? (
                        <p
                          role="status"
                          className="text-sm text-muted-foreground"
                        >
                          Loading warehouse stock...
                        </p>
                      ) : (
                        <div className="space-y-3">
                          <div className="flex flex-wrap gap-x-6 gap-y-1 text-sm">
                            <p>
                              <span className="text-muted-foreground">
                                Article number:{" "}
                              </span>
                              <span className="font-medium">
                                {article.articleNumber}
                              </span>
                            </p>
                            <p>
                              <span className="text-muted-foreground">
                                Unit price:{" "}
                              </span>
                              <span className="font-medium">
                                {currency.format(article.unitPrice)}
                              </span>
                            </p>
                          </div>
                          <div className="flex justify-end">
                            <p className="text-sm text-muted-foreground">
                              Total: {totalQuantity.toLocaleString()}
                            </p>
                          </div>
                          <div className="grid gap-x-6 gap-y-2 sm:grid-cols-2 lg:grid-cols-3">
                            {warehouseStock.data.warehouses.map((warehouse) => (
                              <div
                                key={warehouse.id}
                                className="flex items-center justify-between gap-3 text-sm"
                              >
                                <span className="truncate">
                                  {warehouse.name}
                                </span>
                                <span className="font-medium tabular-nums">
                                  {(
                                    quantitiesByWarehouse.get(warehouse.id) ?? 0
                                  ).toLocaleString()}
                                </span>
                              </div>
                            ))}
                          </div>
                        </div>
                      )}
                    </TableCell>
                  </TableRow>
                )}
              </Fragment>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={2} className="h-24 text-center">
                No articles found.
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
              ? `${data.totalCount.toLocaleString()} articles`
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
